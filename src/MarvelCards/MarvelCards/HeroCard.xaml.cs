using System;
using System.Collections.Generic;
using Xamarin.Forms;
using MarvelCards.Messaging;

namespace MarvelCards
{
    public partial class HeroCard : ContentView
    {
        private CardState _cardState = CardState.Collapsed;

        public HeroCard()
        {
            InitializeComponent();
        }



        public Image CardImage => HeroImage;

        private void LearnMoreLabel_Tapped(object sender, EventArgs e)
        {
            // go to state expanded
            ChangeState(CardState.Expanded);


        }

        private void ChangeState(CardState cardState)
        {
            if (_cardState == cardState) return;

            MessagingCenter.Send<CardEvent>(new CardEvent(), _cardState.ToString());
            AnimateTransition(cardState);

            _cardState = cardState;
        }

        private void AnimateTransition(CardState cardState)
        {

        }
    }

    public enum CardState
    {
        Expanded,
        Collapsed
    }
}
