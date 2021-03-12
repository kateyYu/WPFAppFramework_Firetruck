/*
 * Yu, Katey
 */
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Firetruck
{
    class VM
    {
        public BindingList<string> InputStreetcorners { get; set; } = new BindingList<string>();
        public BindingList<string> OutputRoutes { get; set; } = new BindingList<string>();

        private readonly List<Streetcorner> Streetcorners = new List<Streetcorner>();
        private readonly Dictionary<int, List<int>> Maps = new Dictionary<int, List<int>>();
        private readonly List<string> resultRoutes = new List<string>();
        const int FIRE_STATION = 1;
        private int searchSteps = 20;
        
        public VM()
        {
            InputOutput();
        }
        #region InputOutput
        private void InputOutput()
        {
            string fileName = Properties.Resources.input;
            string[] lines = Regex.Split(fileName, "\r\n");
            string[] NumberPairs;
            Streetcorner streetcorner;
            int firePlace = 0;
            int nCase = 1;
            foreach (string line in lines)
            {
                NumberPairs = line.Split(' ');
                InputStreetcorners.Add(line);
                if (NumberPairs.Length == 1)
                {
                    firePlace = int.Parse(NumberPairs[0]);
                    continue;
                }

                if (int.Parse(NumberPairs[0]) == 0 && int.Parse(NumberPairs[1]) == 0)
                {
                    OutputRoutes.Add("CASE " + nCase++ + ":");
                    CreateMap(firePlace);
                    SearchRoutes(firePlace);
                    resultRoutes.Sort();
                    OutputProduce();
                    OutputRoutes.Add($"There are {resultRoutes.Count()} routes from the firestation to streetcorner {firePlace}.");
                    Streetcorners.Clear();
                    Maps.Clear();
                    resultRoutes.Clear();
                }
                else
                {
                    streetcorner = new Streetcorner()
                    {
                        CurrentStreetcorner = int.Parse(NumberPairs[0]),
                        NextStreetcorner = int.Parse(NumberPairs[1])
                    };
                    Streetcorners.Add(streetcorner);
                }
            }
        }

        #endregion
        #region OutputProduce
        private void OutputProduce()
        {
            foreach (string route in resultRoutes)
            {
                OutputRoutes.Add(route);
            }
        }
        private void AddStreetcorner(int CurrentCorner, int NextCorner)
        {
            int temp;
            if (NextCorner == FIRE_STATION) //change (5,1) into (1,5) 
            {
                temp = CurrentCorner;
                CurrentCorner = NextCorner;
                NextCorner = temp;
            }
            if (Maps.TryGetValue(CurrentCorner, out List<int> fe1))
                fe1.Add(NextCorner);
            else
                Maps.Add(CurrentCorner, new List<int>() { NextCorner });
        }
        #endregion
        #region CreateMap
        private void CreateMap(int firePlace)
        {
            int CurrentCorner, NextCorner;
            foreach (Streetcorner corner in Streetcorners)
            {
                CurrentCorner = corner.CurrentStreetcorner;
                NextCorner = corner.NextStreetcorner;
                AddStreetcorner(CurrentCorner, NextCorner);
                //add another coner
                if (CurrentCorner == FIRE_STATION || NextCorner == FIRE_STATION || NextCorner == firePlace)
                    continue;
                AddStreetcorner(NextCorner, CurrentCorner);
            }
        }
        #endregion
        #region SearchRoutes
        /*
         * Loop time	    CurrentRoutes   ResultRoutes
             0	            1	                0 
             -----------------------------------------
             1              1 2                 0
                            1 3 
            ----------------------------------------
             2	            1 2 3
                            1 2 4
                            1 3 4
                            1 3 5
                            1 3 2	            0
            --------------------------------------------
             3	            1 2 3 4             1 2 4 6
                            1 2 3 5             1 3 5 6
                            1 2 4 3             1 3 5 6
                            1 3 4 2
                            1 3 2 4	        
            ---------------------------------------------                
             4	            1 2 4 3 5           1 2 4 6
                                                1 3 5 6
                                                1 3 5 6
                                                1 2 3 4 6
                                                1 2 3 5 6
                                                1 3 2 4 6
            ----------------------------------------------
             5              0                   1 2 4 6
                                                1 3 5 6
                                                1 3 5 6
                                                1 2 3 4 6
                                                1 2 3 5 6
                                                1 3 2 4 6
                                                1 2 4 3 5 6                                         
              */
        private void SearchRoutes(int firePlace)
        {
            Dictionary<int, List<int>> SortMaps = new Dictionary<int, List<int>>();
            //sort dictionary
            SortMaps = Maps.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            List<int> NextStreets;
            List<string> CurrentRoutes = new List<string>();
            List<string> OldRoutes = new List<string>();
            int nextStreet;
            string currentRoute;
            //initial 
            CurrentRoutes.Add(FIRE_STATION.ToString());
            while (CurrentRoutes.Count() > 0 && searchSteps-- > 0)
            {
                OldRoutes.Clear();
                foreach (string str in CurrentRoutes)
                    OldRoutes.Add(str);
                CurrentRoutes.Clear();
                foreach (string oldRoute in OldRoutes)
                {
                    nextStreet = int.Parse(oldRoute.Substring(oldRoute.Length - 1, 1));
                    NextStreets = SortMaps[nextStreet];
                    foreach (int key in NextStreets)
                    {
                        currentRoute = oldRoute + "   " + key;
                        if (key == firePlace)
                        {
                            resultRoutes.Add(currentRoute);
                            continue;
                        }

                        if (!SortMaps.ContainsKey(key)) continue;
                        //find the key in the string
                        if (oldRoute.Contains(key.ToString())) continue;
                        //add new route string
                        CurrentRoutes.Add(currentRoute);
                    }
                }
            }
        }
        #endregion
        internal class Streetcorner
        {
            public int CurrentStreetcorner { get; set; }
            public int NextStreetcorner { get; set; }
        }
    }
}
