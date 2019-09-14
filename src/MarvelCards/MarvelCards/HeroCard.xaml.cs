using System;
using System.Collections.Generic;
using Xamarin.Forms;
using MarvelCards.Messaging;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;

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


        private float _gradientTransitionY;
        private float _gradientHeight = 200f;


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
            _gradientTransitionY = float.MaxValue;

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

            // draw top card part
            canvas.DrawRoundRect(
                rect: new SKRect(0, (float)_cardTopAnimPosition, info.Width, info.Height),
                r: new SKSize(_cornerRadius, _cornerRadius),
                paint: _heroPaint);

            // draw gradient
            var gradientRect = new SKRect(left: 0,
                top:_gradientTransitionY,
                right: info.Width,
                bottom: _gradientTransitionY + _gradientHeight);
            var gradientPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Shader = SKShader.CreateLinearGradient(
                    start: new SKPoint(0, _gradientTransitionY),
                    end: new SKPoint(0, _gradientTransitionY + _gradientHeight),
                    colors: new SKColor[] { _heroColor, SKColors.White },
                    colorPos: new float[] { 0, 1 },
                    mode: SKShaderTileMode.Clamp)
            };
            canvas.DrawRect(gradientRect, gradientPaint);


            // draw bottom of card
            canvas.DrawRect(
                rect: new SKRect(0, _gradientTransitionY+_gradientHeight, info.Width, info.Height),
                paint: new SKPaint { Color = SKColors.White });



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
                parentAnimation.Add(0.0,  0.15, CreateCardAnimation(cardState));
                parentAnimation.Add(0.0,  0.5, CreateHeroImageAnimation(cardState));
                parentAnimation.Add(0.0,  0.5, CreateHeroNameAnimation(cardState));
                parentAnimation.Add(0.05, 0.5, CreateRealNameAnimation(cardState));
                parentAnimation.Add(0.0, 0.3, CreateLearnMoreAnimation(cardState));
                parentAnimation.Add(0.5, 0.75, CreateGradientAnimation(cardState));
            }
            else
            {
                parentAnimation.Add(0.0, 0.3, CreateGradientAnimation(cardState));
                parentAnimation.Add(0.2, 0.6, CreateCardAnimation(cardState));
                parentAnimation.Add(0.2, 0.6, CreateHeroImageAnimation(cardState));
                parentAnimation.Add(0.3, 0.6, CreateHeroNameAnimation(cardState));
                parentAnimation.Add(0.3, 0.6, CreateRealNameAnimation(cardState));
                parentAnimation.Add(0.3, 0.6, CreateLearnMoreAnimation(cardState));
            }

            parentAnimation.Commit(this, "MoreInfoAnimation", length:2000);

        }

        private Animation CreateGradientAnimation(CardState cardState)
        {
            float animStart, animEnd;

            Debug.WriteLine($"Canvass height: {CardBackground.CanvasSize.Height}");
            if (cardState == CardState.Expanded)
            {
                _gradientTransitionY = CardBackground.CanvasSize.Height;
                animStart = _gradientTransitionY;
                animEnd = -_gradientHeight;
                
            }
            else
            {
                _gradientTransitionY = -_gradientHeight;
                animStart = -_gradientTransitionY;
                animEnd = CardBackground.CanvasSize.Height;
            }

            var anim = new Animation(
                callback: v =>
                {
                    _gradientTransitionY = (float)v;
                    CardBackground.InvalidateSurface();
                },
                start: animStart,
                end: animEnd,
                finished: () =>
                {
                    var fontColor = cardState == CardState.Expanded ? Color.Black : Color.White;
                    HeroNameLabelLine1.TextColor = fontColor;
                    HeroNameLabelLine2.TextColor = fontColor;
                    RealNameLabel.TextColor = fontColor;
                }
                );

            return anim;
        }


        private Animation CreateLearnMoreAnimation(CardState cardState)
        {
            double animStart, animEnd;
            
            if (cardState == CardState.Expanded)
            {
                animStart = 1;
                animEnd = 0;
            }
            else
            {
                animStart = 0;
                animEnd = 1;

            }

            var cardAnim = new Animation(
                v =>
                {
                    LearnMoreLabel.Opacity = v;
                    LearnMoreLabel.TranslationX = (1-v)*200;
                },
                animStart,
                animEnd);

            return cardAnim;
        }

        private Animation CreateRealNameAnimation(CardState cardState)
        {
            double animStart, animEnd;
            Easing easing;
            if (cardState == CardState.Expanded)
            {
                animStart = RealNameLabel.TranslationY;
                animEnd = -50;
                easing = Easing.SpringOut;
            }
            else
            {
                animStart = RealNameLabel.TranslationY;
                animEnd = 0;
                easing = Easing.SpringIn;

            }

            var cardAnim = new Animation(
                v =>
                {
                    RealNameLabel.TranslationY = v;
                },
                animStart,
                animEnd,
                easing);

            return cardAnim;
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
