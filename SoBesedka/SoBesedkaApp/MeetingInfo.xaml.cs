﻿using SoBesedkaDB.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SoBesedkaApp
{
    /// <summary>
    /// Логика взаимодействия для MeetingInfo.xaml
    /// </summary>
    public partial class MeetingInfo : Window
    {
        MeetingViewModel Meeting;
        public MeetingInfo(MeetingViewModel meeting)
        {
            Meeting = meeting;
            DataContext = Meeting;
            InitializeComponent();
        }
    }
}
