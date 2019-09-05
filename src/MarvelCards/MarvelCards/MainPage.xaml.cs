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
        const double _defaultTranslationY = -150;

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
            MessagingCenter.Subscribe<CardEvent>(this, CardState.Expanded.ToString(), CardExpand);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MainCardView.UserInteracted -= MainCardView_UserInteracted;
            MessagingCenter.Unsubscribe<CardEvent>(this, CardState.Expanded.ToString());

        }

        private void CardExpand(CardEvent message)
        {
            // turn off swipe direction
            MainCardView.IsUserInteractionEnabled = false;

        }

        private void MainCardView_UserInteracted(PanCardView.CardsView view,
            PanCardView.EventArgs.UserInteractedEventArgs args)
        {
            var card = MainCardView.CurrentView as HeroCard;
            var nextCard = MainCardView.CurrentBackViews.FirstOrDefault() as HeroCard;

            var percentFromCenter = Math.Abs(args.Diff / this.Width);
            Debug.WriteLine($"% from C: {card.CardImage.Source.ToString()} {args.Status} {percentFromCenter}");

            if (args.Status == PanCardView.Enums.UserInteractionStatus.Started)
            {
                if (nextCard != null && false)
                {
                    nextCard.Opacity = 1;
                    nextCard.CardImage.Scale = 1;
                    nextCard.CardImage.TranslationY = _defaultTranslationY;
                }
            }

            if (args.Status == PanCardView.Enums.UserInteractionStatus.Running)
            {
                // control opacity of currnet card
                var opacity = 1 - (percentFromCenter * 0.8);
                card.Opacity = (opacity > 1) ? 1 : opacity;

                // set scale of image in current card
                card.CardImage.Scale = Math.Max(1 - (percentFromCenter * 1.5), .5);

                // set position of image in current card
                var movementFactor = 150;
                card.CardImage.TranslationY = _defaultTranslationY + (movementFactor * percentFromCenter);

                // set opacity of next card
                nextCard.CardImage.Opacity = 1 - (opacity/4);
                nextCard.CardImage.Scale = Math.Min(percentFromCenter * 3, 1);

                // set margin
                var lrMargin = Math.Min(10, 50 * percentFromCenter);

                // add margin
                card.ScaleTo(.9, 50);

            }

            if(args.Status == PanCardView.Enums.UserInteractionStatus.Ended
                || args.Status == PanCardView.Enums.UserInteractionStatus.Ending)
            {
                card.Opacity = 1;
                card.CardImage.Opacity = 1;
                card.CardImage.Scale = 1;
                card.CardImage.TranslationY = _defaultTranslationY;

                // ensure next card is hidden
                nextCard.CardImage.Opacity = 0;
                nextCard.CardImage.Scale = 1;
                nextCard.CardImage.TranslationY = _defaultTranslationY;

                // remove margin
                card.ScaleTo(1, 50);

            }
        }

        
    }
}
