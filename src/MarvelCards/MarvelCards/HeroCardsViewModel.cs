using System;
using System.Collections.ObjectModel;

namespace MarvelCards
{
    public class HeroCardsViewModel
    {
        public HeroCardsViewModel()
        {

        }

        public ObservableCollection<Hero> Heroes { get; set; }
            = new ObservableCollection<Hero>
            {
                new Hero
                {
                    HeroColor="#DF8E04",
                    HeroNameLine1="iron",
                    HeroNameLine2="man",
                    Image="ironman.png",
                    RealName="tony stark"
                },
                new Hero
                {
                    HeroColor="#C51925",
                    HeroNameLine1="spider",
                    HeroNameLine2="man",
                    Image="spiderman.png",
                    RealName="peter parker"
                },
            };


    }

    public class Hero
    {
        public string HeroNameLine1 { get; set; }
        public string HeroNameLine2 { get; set; }
        public string RealName { get; set; }
        public string Image { get; set; }
        public string HeroColor { get; set; }

    }

}
