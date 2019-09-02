using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MarvelCards
{
    public partial class HeroCard : ContentView
    {
        public HeroCard()
        {
            InitializeComponent();
        }

        public Image CardImage => HeroImage;
        
    }
}
