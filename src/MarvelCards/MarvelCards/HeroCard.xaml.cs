using System;
using System.Collections.Generic;
using Xamarin.Forms;
using MarvelCards.Messaging;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms.Xaml;

namespace MarvelCards
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeroCard : ContentView
    {
        private Hero _viewModel;
        private CardState _cardState = CardState.Collapsed;
        private readonly float _density;
        private readonly float _cardTopMargin;
        private readonly float _cornerRadius = 60f;
        private double _cardTopAnimPosition;


        SKColor _heroColor;
        SKPaint _heroPaint;


        public HeroCard()
        {
            InitializeComponent();

            _density = (float)Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;
            _cardTopMargin = 200f * _density;
        }

        public Image MainImage => HeroImage;

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if(this.BindingContext == null) return;

            _viewModel = this.BindingContext as Hero;

            _heroColor = Color.FromHex(_viewModel.HeroColor).ToSKColor();
            _heroPaint = new SKPaint { Color = _heroColor };

            // setup initial values
            _cardTopAnimPosition = _cardTopMargin;

            // repaint surface with new colors
            CardBackground.InvalidateSurface(); 
        }

        

        public void ChangeState(CardState cardState)
        {
            if (_cardState == cardState) return;
            _cardState = cardState;

            MessagingCenter.Send<CardEvent>(new CardEvent(), _cardState.ToString());
            AnimateTransition(cardState);
        }

        private void CardBackground_PaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            if (_viewModel == null) return;

            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            // draw card background
            canvas.DrawRoundRect(
                rect: new SKRect(0, (float)_cardTopAnimPosition, info.Width, info.Height),
                r: new SKSize(_cornerRadius, _cornerRadius),
                paint: _heroPaint);


        }

        private void LearnMoreLabel_Tapped(object sender, EventArgs e)
        {
            // go to state expanded
            ChangeState(CardState.Expanded);

        }

        private void AnimateTransition(CardState cardState)
        {
            var parentAnimation = new Animation();

            if(cardState == CardState.Expanded)
            {
                parentAnimation.Add(0, .15, CreateCardAnimation(cardState));
                parentAnimation.Add(0, .5, CreateHeroImageAnimation(cardState));
                parentAnimation.Add(0, .5, CreateHeroNameAnimation(cardState));
            }
            else
            {
                parentAnimation.Add(0.1, 0.4, CreateCardAnimation(cardState));
                parentAnimation.Add(0.0, 0.4, CreateHeroImageAnimation(cardState));
                parentAnimation.Add(0.1, 0.4, CreateHeroNameAnimation(cardState));
            }

            parentAnimation.Commit(this, "MoreInfoAnimation", length:2000);

        }

        private Animation CreateHeroNameAnimation(CardState cardState)
        {
            double animStart, animEnd;
            Easing easing;
            if (cardState == CardState.Expanded)
            {
                animStart = HeroNameLabels.TranslationY;
                animEnd = -50;
                easing = Easing.SpringOut;
            }
            else
            {
                animStart = HeroNameLabels.TranslationY;
                animEnd = 0;
                easing = Easing.SpringIn;

            }

            var cardAnim = new Animation(
                v =>
                {
                    HeroNameLabels.TranslationY = v;
                    RealNameLabel.TranslationY = v;
                },
                animStart,
                animEnd,
                easing);

            return cardAnim;
        }

        private Animation CreateHeroImageAnimation(CardState cardState)
        {
            double animStart, animEnd;
            Easing easing;
            if(cardState == CardState.Expanded)
            {
                animStart = HeroImage.TranslationY;
                animEnd = 0;
                easing = Easing.SpringOut;
            }
            else
            {
                animStart = HeroImage.TranslationY;
                animEnd = 50;
                easing = Easing.SpringIn;

            }
            
            var cardAnim = new Animation(
                v =>
                {
                    HeroImage.TranslationY = v;
                },
                animStart,
                animEnd,
                easing);

            return cardAnim;
        }

        private Animation CreateCardAnimation(CardState cardState)
        {
            var cardAnimStart = cardState == CardState.Expanded ? _cardTopMargin : -_cornerRadius;
            var cardAnimEnd = cardState == CardState.Expanded ? -_cornerRadius : _cardTopMargin;

            var cardAnim = new Animation(
                v=>
                {
                    _cardTopAnimPosition = v;
                    CardBackground.InvalidateSurface();

                },
                cardAnimStart,
                cardAnimEnd,
                Easing.SinInOut);

            return cardAnim;
        }
    }

    public enum CardState
    {
        Expanded,
        Collapsed
    }
}
