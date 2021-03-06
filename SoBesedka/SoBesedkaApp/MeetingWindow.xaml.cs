﻿using SoBesedkaDB;
using SoBesedkaDB.Implementations;
using SoBesedkaDB.Interfaces;
using SoBesedkaDB.Views;
using SoBesedkaModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MaskedTextBox = System.Windows.Forms.MaskedTextBox;
using DockStyle = System.Windows.Forms.DockStyle;
using BorderStyle = System.Windows.Forms.BorderStyle;

namespace SoBesedkaApp
{
    /// <summary>
    /// Логика взаимодействия для EventWindow.xaml
    /// </summary>
    public partial class MeetingWindow : Window
    {
        DataSamples Data;
        MeetingViewModel Meeting;
        List<UserViewModel> InvitedUsers;

        private MaskedTextBox startTimeMaskedTextBox;
        private MaskedTextBox durationMaskedTextBox;

        public MeetingWindow(DataSamples data, MeetingViewModel meeting)
        {
            InitializeComponent();
            startTimeMaskedTextBox = new MaskedTextBox("00:00")
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Width = (int)host1.Width,
                Height = (int)host1.Height,
                Font = new System.Drawing.Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle
            };
            host1.Child = startTimeMaskedTextBox;

            durationMaskedTextBox = new MaskedTextBox("00:00")
            {
                Dock = DockStyle.Fill,
                Width = (int)host2.Width,
                Height = (int)host2.Height,
                Font = new System.Drawing.Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle
            };
            host2.Child = durationMaskedTextBox;

            Meeting = meeting;
            Data = data;
            DataContext = data;
            TitleTextBox.Text = Meeting.MeetingName;
            SubjTextBox.Text = Meeting.MeetingTheme;
            DescriptionTextBox.Text = Meeting.MeetingDescription;
            DatePicker.SelectedDate = Meeting.StartTime.Date;
            int i = 0;
            if (Meeting.RepeatingDays != null)
                foreach (CheckBox cb in CheckBoxContainer.Children)
                {
                    if (Meeting.RepeatingDays[i] == '1')
                        cb.IsChecked = true;
                    i++;
                }
            Data.UpdateUsers();
            if (Meeting.UserMeetings != null)
                foreach (UserMeetingViewModel um in Meeting.UserMeetings)
                {
                    var user = Data.Users.Find(u => u.Id == um.UserId);
                    if (user != null)
                        InvitedUsersListBox.Items.Add(user);
                }

            startTimeMaskedTextBox.Text = meeting.StartTime.ToString("HH\\:mm");

            var t = (meeting.EndTime - meeting.StartTime).ToString("c");
            durationMaskedTextBox.Text = t;
            if (Meeting.Id == 0)
            {
                DelButton.Visibility = Visibility.Hidden;
                Title = "Добавление";
            }
            else
                Title = "Изменение";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var repDays = "";
                foreach (CheckBox cb in CheckBoxContainer.Children)
                {
                    if (cb.IsChecked == null) continue;
                    if (cb.IsChecked.Value)
                    {
                        repDays += "1";
                    }
                    else
                    {
                        repDays += "0";
                    }
                }
                var userMeetings = new List<UserMeeting>();
                foreach (UserViewModel user in InvitedUsersListBox.Items)
                {
                    if (Meeting.Id > 0)
                        userMeetings.Add(new UserMeeting
                        {
                            UserId = user.Id,
                            MeetingId = Meeting.Id
                        });
                    else
                        userMeetings.Add(new UserMeeting
                        {
                            UserId = user.Id
                        });
                }
                if (Meeting.Id > 0)
                {
                    //Изменение
                    if (DatePicker.SelectedDate != null &&
                        !string.IsNullOrEmpty(startTimeMaskedTextBox.Text) &&
                        !string.IsNullOrEmpty(durationMaskedTextBox.Text) &&
                        !string.IsNullOrEmpty(TitleTextBox.Text) &&
                        !string.IsNullOrEmpty(SubjTextBox.Text) &&
                        !string.IsNullOrEmpty(DescriptionTextBox.Text))
                    {
                        if (DatePicker.SelectedDate.Value + DateTime.Parse(startTimeMaskedTextBox.Text).TimeOfDay <= DateTime.Now) {
                            MessageBox.Show("Время, на которое Вы хотите назвачить мероприятие, уже прошло", "Ошибка", MessageBoxButton.OK);
                            return;
                        }
                        var response = APIClient.PostRequest("api/Meeting/UpdElement", new Meeting
                        {
                            Id = Meeting.Id,
                            MeetingName = TitleTextBox.Text,
                            MeetingTheme = SubjTextBox.Text,
                            MeetingDescription = DescriptionTextBox.Text,
                            StartTime = DatePicker.SelectedDate.Value + DateTime.Parse(startTimeMaskedTextBox.Text).TimeOfDay,
                            EndTime = DatePicker.SelectedDate.Value + DateTime.Parse(startTimeMaskedTextBox.Text).TimeOfDay +
                                      DateTime.Parse(durationMaskedTextBox.Text).TimeOfDay,
                            UserMeetings = userMeetings,
                            RoomId = Data.CurrentRoom.Id,
                            CreatorId = Data.CurrentUser.Id,
                            RepeatingDays = repDays
                        });
                        MessageBox.Show("Изменено", "Успех", MessageBoxButton.OK);
                    }
                }
                else
                {
                    //Добавление
                    if (DatePicker.SelectedDate != null &&
                        !string.IsNullOrEmpty(startTimeMaskedTextBox.Text) &&
                        !string.IsNullOrEmpty(durationMaskedTextBox.Text) &&
                        !string.IsNullOrEmpty(TitleTextBox.Text) &&
                        !string.IsNullOrEmpty(SubjTextBox.Text) &&
                        !string.IsNullOrEmpty(DescriptionTextBox.Text))
                    {
                        if (DatePicker.SelectedDate.Value + DateTime.Parse(startTimeMaskedTextBox.Text).TimeOfDay <= DateTime.Now)
                        {
                            MessageBox.Show("Время, на которое Вы хотите назвачить мероприятие, уже прошло", "Ошибка", MessageBoxButton.OK);
                            return;
                        }
                        Data.AddElement(new Meeting
                        {
                            MeetingName = TitleTextBox.Text,
                            MeetingTheme = SubjTextBox.Text,
                            MeetingDescription = DescriptionTextBox.Text,
                            StartTime = DatePicker.SelectedDate.Value + DateTime.Parse(startTimeMaskedTextBox.Text).TimeOfDay,
                            EndTime = DatePicker.SelectedDate.Value + DateTime.Parse(startTimeMaskedTextBox.Text).TimeOfDay +
                                      DateTime.Parse(durationMaskedTextBox.Text).TimeOfDay,
                            UserMeetings = userMeetings,
                            RoomId = Data.CurrentRoom.Id,
                            CreatorId = Data.CurrentUser.Id,
                            RepeatingDays = repDays
                        });
                    }
                    MessageBox.Show("Добавлено", "Успех", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OKCancel);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var users = new AddUsers(Data);
            foreach (var user in InvitedUsersListBox.Items)
            {
                users.SelectedUsers.Add((UserViewModel)user);
            }
            users.OnPropertyChanged("SelectedUsers");
            users.SelectedUsersListBox.Items.Refresh();
            if (users.ShowDialog() == true)
            {
                InvitedUsersListBox.Items.Clear();
                foreach(var user in users.SelectedUsers)
                {
                    InvitedUsersListBox.Items.Add(user);
                }
            }
        }

        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Data.DelElement(Meeting);
                MessageBox.Show("Удалено", "Успех", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OKCancel);
            }
            DialogResult = true;
            Close();
        }
    }
}