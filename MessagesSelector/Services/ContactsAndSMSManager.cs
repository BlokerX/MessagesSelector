﻿using Android;
using Android.App;
using Android.Content.PM;
using AndroidX.Core.App;
using Java.Util;
using MessagesSelector.Items;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using Xamarin.Essentials;

namespace MessagesSelector.Services
{
    internal class ContactsAndSMSManager
    {
        public bool RequestObligatoryPermissions(Activity activity)
        {
            Permission a, b, c;

            a = ActivityCompat.CheckSelfPermission(activity, "android.permission.READ_SMS");
            b = ActivityCompat.CheckSelfPermission(activity, "android.permission.READ_CALL_LOG");
            c = ActivityCompat.CheckSelfPermission(activity, "android.permission.READ_CONTACTS");

            while (a == Permission.Denied ||
               b == Permission.Denied ||
               c == Permission.Denied)
            {
                ActivityCompat.RequestPermissions(activity, new String[] {
                    Manifest.Permission.ReadSms,
                    Manifest.Permission.ReadCallLog,
                    Manifest.Permission.ReadContacts
                    }, 1);

                a = ActivityCompat.CheckSelfPermission(activity, "android.permission.READ_SMS");
                b = ActivityCompat.CheckSelfPermission(activity, "android.permission.READ_CALL_LOG");
                c = ActivityCompat.CheckSelfPermission(activity, "android.permission.READ_CONTACTS");
            }

            return true;
        }

        /// <summary>
        /// Messages.
        /// </summary>
        readonly ObservableCollection<Message> messages = new ObservableCollection<Message>();

        /// <summary>
        /// Contacts.
        /// </summary>

        readonly ObservableCollection<Contact> contactsCollect = new ObservableCollection<Contact>();

        public void GetAllSms(Activity activity)
        {
            RequestObligatoryPermissions(activity);

            string INBOX = "content://sms/inbox";
            string[] reqCols = new string[] { "_id", "thread_id", "address", "person", "date", "body", "type" };
            Android.Net.Uri uri = Android.Net.Uri.Parse(INBOX);
            var cursor = Application.Context.ContentResolver.Query(uri, reqCols, null, null, null);

            if (cursor.MoveToFirst())
            {
                do
                {
                    string messageId = cursor.GetString(cursor.GetColumnIndex(reqCols[0]));
                    string threadId = cursor.GetString(cursor.GetColumnIndex(reqCols[1]));
                    string address = cursor.GetString(cursor.GetColumnIndex(reqCols[2]));
                    string name = cursor.GetString(cursor.GetColumnIndex(reqCols[3]));
                    Date date = new Date(cursor.GetLong(4));
                    string msg = cursor.GetString(cursor.GetColumnIndex(reqCols[5]));
                    string type = cursor.GetString(cursor.GetColumnIndex(reqCols[6]));

                    messages.Add(new Message(messageId, threadId, address, name, date, msg, type, contactsCollect));

                } while (cursor.MoveToNext());

            }
            DebugWriteSMS();
        }

        #region Call

        public async void GetAllContactsAsync()
        {

            try
            {
                // cancellationToken parameter is optional
                var cancellationToken = default(CancellationToken);
                var contacts = await Xamarin.Essentials.Contacts.GetAllAsync(cancellationToken);

                if (contacts == null)
                    return;

                foreach (var contact in contacts)
                    contactsCollect.Add(contact);
            }
            catch
            {
                // Handle exception here.
            }
            DebugWriteContacts();
        }

        #endregion

#if DEBUG
        public void DebugWriteSMS()
        {
            if (messages != null)
                foreach (var item in messages)
                {
                    System.Diagnostics.Debug.WriteLine("-------- Message --------");
                    System.Diagnostics.Debug.WriteLine("ID: " + item.MessageId);
                    System.Diagnostics.Debug.WriteLine("Thread ID: " + item.ThreadId);
                    System.Diagnostics.Debug.WriteLine("Address: " + item.Address);
                    System.Diagnostics.Debug.WriteLine("Person: " + item.Person);
                    System.Diagnostics.Debug.WriteLine("Date: " + item.Date);
                    System.Diagnostics.Debug.WriteLine("Text: " + item.Text);
                    System.Diagnostics.Debug.WriteLine("Type: " + item.Type);
                    System.Diagnostics.Debug.WriteLine("-------------------------");
                }
        }

        public void DebugWriteContacts()
        {
            if (contactsCollect != null)
                foreach (var item in contactsCollect)
                {
                    string phones = string.Empty;
                    foreach (var phone in item.Phones)
                    {
                        phones += phone.PhoneNumber + ", ";
                    }
                    System.Diagnostics.Debug.WriteLine("+++++++++++ Number +++++++++++");
                    System.Diagnostics.Debug.WriteLine(item.DisplayName + " " + phones);
                    System.Diagnostics.Debug.WriteLine("++++++++++++++++++++++++++++++");
                }
        }
#endif

    }
}