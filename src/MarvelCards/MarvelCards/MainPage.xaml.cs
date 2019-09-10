using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarvelCards.Messaging;
using Xamarin.Forms;

namespace MarvelCards
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        const double _defaultTranslationY = 50;
        const double _movementFactor = 100;

        public MainPage()
        {
            InitializeComponent();
            base.BindingContext = new HeroCardsViewModel();            
        }
         
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MainCardView.UserInteracted += MainCardView_UserInteracted;
            // subscribe to messaging
            MessagingCenter.Subscribe<CardEvent>(this, CardState.Expanded.ToString(), CardExpanded);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MainCardView.UserInteracted -= MainCardView_UserInteracted;
            MessagingCenter.Unsubscribe<CardEvent>(this, CardState.Expanded.ToString());

        }

        private void CardExpanded(CardEvent message)
        {
            // do animation
            AnimateTitle(CardState.Expanded);

            // turn on user interaction
            MainCardView.IsUserInteractionEnabled = false;
        }

        private void BackArrowImage_Tapped(object sender, EventArgs e)
        {
            AnimateTitle(CardState.Collapsed);

            // communicate change to card view
            ((HeroCard)MainCardView.CurrentView).ChangeState(CardState.Collapsed);

            MainCardView.IsUserInteractionEnabled = true;
        }

        private void AnimateTitle(CardState cardState)
        {
            var translationY = cardState == CardState.Expanded
                ? -1 * (MoviesHeader.Height + MoviesHeader.Margin.Top)
                : 0;

            var opacity = cardState == CardState.Expanded ? 0 : 1;
                        
            var animation = new Animation
            {
                { 0, 1, new Animation(v => MoviesHeader.TranslationY = v, MoviesHeader.TranslationY, translationY) },
                { 0, 1, new Animation(v => MoviesHeader.Opacity = v, MoviesHeader.Opacity, opacity) }
            };
            animation.Commit(this, "titleAnimation");
        }

        private void MainCardView_UserInteracted(PanCardView.CardsView view,
            PanCardView.EventArgs.UserInteractedEventArgs args)
        {
            var card = MainCardView.CurrentView as HeroCard;
            var nextCard = MainCardView.CurrentBackViews.FirstOrDefault() as HeroCard;

            var percentFromCenter = Math.Abs(args.Diff / this.Width);
            Debug.WriteLine($"% from C: {card.MainImage.Source.ToString()} {args.Status} {percentFromCenter}");

            if (args.Status == PanCardView.Enums.UserInteractionStatus.Started)
            {
                if (nextCard != null && false)
                {
                    nextCard.Opacity = 1;
                    nextCard.MainImage.Scale = 1;
                    nextCard.MainImage.TranslationY = _defaultTranslationY;
                }
            }

            if (args.Status == PanCardView.Enums.UserInteractionStatus.Running)
            {
                // control opacity of currnet card
                var opacity = 1 - (percentFromCenter * 0.8);
                card.Opacity = (opacity > 1) ? 1 : opacity;

                // set scale of image in current card
                card.MainImage.Scale = Math.Max(1 - (percentFromCenter * 1.5), .5);

                // set position of image in current card
                var movementFactor = 150;
                card.MainImage.TranslationY = _defaultTranslationY + (movementFactor * percentFromCenter);

                // set opacity of next card
                nextCard.MainImage.Opacity = 1 - (opacity/4);
                nextCard.MainImage.Scale = Math.Min(percentFromCenter * 3, 1);

                // set margin
                var lrMargin = Math.Min(10, 50 * percentFromCenter);

                // add margin
                card.ScaleTo(.9, 50);

            }

            if(args.Status == PanCardView.Enums.UserInteractionStatus.Ended
                || args.Status == PanCardView.Enums.UserInteractionStatus.Ending)
            {
                card.Opacity = 1;
                card.MainImage.Opacity = 1;
                card.MainImage.Scale = 1;
                card.MainImage.TranslationY = _defaultTranslationY;

                // ensure next card is hidden
                nextCard.MainImage.Opacity = 0;
                nextCard.MainImage.Scale = 1;
                nextCard.MainImage.TranslationY = _defaultTranslationY;

                // remove margin
                card.ScaleTo(1, 50);

            }
        }
    }
}
