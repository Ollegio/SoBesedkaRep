﻿using SoBesedkaDB;
using SoBesedkaDB.Implementations;
using SoBesedkaDB.Interfaces;
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
    /// Логика взаимодействия для EventWindow.xaml
    /// </summary>
    public partial class MeetingWindow : Window
    {
        DataSamples Data;
        IMeetingService Mservice;
        public MeetingWindow(DataSamples data)
        {
            InitializeComponent();
            Data = data;
            DataContext = data;
            Mservice = new MeetingService(new SoBesedkaDBContext());
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Mservice.AddElement(new SoBesedkaModels.Meeting
            {
                MeetingName = TitleTextBox.Text,
                MeetingTheme = SubjTextBox.Text,
                MeetingDescription = DescriptionTextBox.Text,
                StartTime = DatePicker.SelectedDate.Value + DateTime.Parse(TimeStartTextBox.Text).TimeOfDay,
                EndTime = DatePicker.SelectedDate.Value + DateTime.Parse(TimeStartTextBox.Text).TimeOfDay + DateTime.Parse(DlitTextBox.Text).TimeOfDay,
                UserMeetings = null,
                RoomId = Data.CurrentRoom.Id,
                CreatorId = Data.CurrentUser.Id
            });
            Close();
            Data.UpdateMeetings();
            Data.Uservice.SendEmail(Data.CurrentUser.UserMail, "Оповещение о создании мероприятия",
                String.Format("Мероприятие успешно добавлено. \n Название: {0}. \n Тема: {1}. \n Время: {2}. \n Место: {3}", TitleTextBox.Text, SubjTextBox.Text, TimeStartTextBox.Text, Data.CurrentRoom.RoomName));

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
