using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using MessagesSelector.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Message = MessagesSelector.Items.Message;

namespace MessagesSelector
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            searchButton = FindViewById<Android.Widget.Button>(Resource.Id.search_button);
            searchButton.Click += Button1_Click;

            searchTextBox = FindViewById<Android.Widget.EditText>(Resource.Id.search_text_box);

            dateCheckBox = FindViewById<Android.Widget.CheckBox>(Resource.Id.date_check_box);

            dateCheckBox.CheckedChange += DateCheckBox_CheckedChange;

            #region Date picker

            searchDatePicker = FindViewById<Android.Widget.LinearLayout>(Resource.Id.date_panel);

            #region Min date

            searchMinDateYearNumberPicker = FindViewById<Android.Widget.NumberPicker>(Resource.Id.search_min_date_picker_year);
            searchMinDateYearNumberPicker.MinValue = 1960;
            searchMinDateYearNumberPicker.MaxValue = 2050;
            searchMinDateYearNumberPicker.Value = 1960;

            searchMinDateMonthNumberPicker = FindViewById<Android.Widget.NumberPicker>(Resource.Id.search_min_date_picker_month);
            searchMinDateMonthNumberPicker.MinValue = 1;
            searchMinDateMonthNumberPicker.MaxValue = 12;
            searchMinDateMonthNumberPicker.Value = 1;

            searchMinDateDayNumberPicker = FindViewById<Android.Widget.NumberPicker>(Resource.Id.search_min_date_picker_day);
            searchMinDateDayNumberPicker.MinValue = 1;
            searchMinDateDayNumberPicker.MaxValue = 31;
            searchMinDateDayNumberPicker.Value = 1;

            #endregion

            #region Max date
            searchMaxDateYearNumberPicker = FindViewById<Android.Widget.NumberPicker>(Resource.Id.search_max_date_picker_year);
            searchMaxDateYearNumberPicker.MinValue = 1960;
            searchMaxDateYearNumberPicker.MaxValue = 2050;
            searchMaxDateYearNumberPicker.Value = DateTime.UtcNow.Year;

            searchMaxDateMonthNumberPicker = FindViewById<Android.Widget.NumberPicker>(Resource.Id.search_max_date_picker_month);
            searchMaxDateMonthNumberPicker.MinValue = 1;
            searchMaxDateMonthNumberPicker.MaxValue = 12;
            searchMaxDateMonthNumberPicker.Value = DateTime.UtcNow.Month;

            searchMaxDateDayNumberPicker = FindViewById<Android.Widget.NumberPicker>(Resource.Id.search_max_date_picker_day);
            searchMaxDateDayNumberPicker.MinValue = 1;
            searchMaxDateDayNumberPicker.MaxValue = 31;
            searchMaxDateDayNumberPicker.Value = DateTime.UtcNow.Day;

            #endregion

            #endregion

            messagesListView = FindViewById<Android.Widget.ListView>(Resource.Id.messages_list);
            messagesListView.ItemClick += (s, e) =>
            {
                Android.Widget.Toast.MakeText(this, messagesObservableCollection[e.Position].Text, Android.Widget.ToastLength.Long).Show();
            };

            contactsAndSMSManager = new ContactsAndSMSManager();

            #region pre load

            ActivityCompat.RequestPermissions(this, new String[] {
                    Manifest.Permission.ReadSms,
                    Manifest.Permission.ReadCallLog,
                    Manifest.Permission.ReadContacts
                    }, 1);
            contactsAndSMSManager.GetAllContactsAsync();

            #endregion

        }

        private void DateCheckBox_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs e)
        {
            switch (e.IsChecked)
            {
                case true:
                    searchDatePicker.Visibility = ViewStates.Visible;
                    break;

                case false:
                    searchDatePicker.Visibility = ViewStates.Gone;
                    break;
            }
        }

        Android.Widget.EditText searchTextBox;

        Android.Widget.CheckBox dateCheckBox;


        Android.Widget.LinearLayout searchDatePicker;

        #region Min Date
        Android.Widget.NumberPicker searchMinDateYearNumberPicker;
        Android.Widget.NumberPicker searchMinDateMonthNumberPicker;
        Android.Widget.NumberPicker searchMinDateDayNumberPicker;
        #endregion


        #region Max Date
        Android.Widget.NumberPicker searchMaxDateYearNumberPicker;
        Android.Widget.NumberPicker searchMaxDateMonthNumberPicker;
        Android.Widget.NumberPicker searchMaxDateDayNumberPicker;
        #endregion

        Android.Widget.Button searchButton;

        Android.Widget.ListView messagesListView;

        // ---------------------------------------- //
        private ContactsAndSMSManager contactsAndSMSManager;

        private ObservableCollection<Message> messagesObservableCollection;

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //sort and filtr
            if (dateCheckBox.Checked)
#pragma warning disable CS0618 // Typ lub składowa jest przestarzała
                messagesObservableCollection = contactsAndSMSManager.GetMessagesByFiltr(
                    this,
                    searchTextBox.Text, 
                    new Java.Util.Date(searchMinDateYearNumberPicker.Value - 1900, searchMinDateMonthNumberPicker.Value - 1, searchMinDateDayNumberPicker.Value),
                    new Java.Util.Date(searchMaxDateYearNumberPicker.Value - 1900, searchMaxDateMonthNumberPicker.Value - 1, searchMaxDateDayNumberPicker.Value, 23, 59,59)
                    );
#pragma warning restore CS0618 // Typ lub składowa jest przestarzała
            else
                messagesObservableCollection = contactsAndSMSManager.GetMessagesByFiltr(this, searchTextBox.Text);

            if (messagesObservableCollection != null)
            {
                List<string> strings = new List<string>();
                foreach (var m in messagesObservableCollection)
                {
                    if (m.Person != null)
                        strings.Add("[" + m.Person + "] : " + m.Text);
                    else
                        strings.Add("[" + m.Address + "] : " + m.Text);
                }
                messagesListView.Adapter = new Android.Widget.ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, strings);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
