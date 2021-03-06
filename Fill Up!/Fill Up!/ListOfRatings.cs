﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Fill_Up_
{
    public partial class ListOfRatings : Form
    {
        ListsOfBars allBars;

        public ListOfRatings(ListsOfBars a)
        {
            allBars = a;
            InitializeComponent();

            ListViewItem itm;
            List <RatedBar> x = allBars.GetRatings();
            string[] info = new string[2];

            foreach (RatedBar bar in x)
            {
                info[0] = bar.Name;
                info[1] = bar.Rating.ToString();
                itm = new ListViewItem(info);
                listView1.Items.Add(itm);
            }             
        }

        private void ListOfRatings_Load(object sender, EventArgs e)
        {

        }
    }
}
