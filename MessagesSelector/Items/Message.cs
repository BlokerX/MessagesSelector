using Java.Util;
using System.Collections.ObjectModel;
using Xamarin.Essentials;

namespace MessagesSelector.Items
{
    internal class Message
    {
        public Message(string messageId, string threadId, string address, string person, Date date, string text, string type, ObservableCollection<Contact> contacts)
        {
            MessageId = messageId;
            ThreadId = threadId;
            Address = address;
            Person = person;
            Date = date;
            Text = text;
            Type = type;

            if (Person is null)
            {
                foreach (var contact in contacts)
                {
                    foreach (var phone in contact.Phones)
                    {
                        // todo phone number convert to universal
                        if (address == phone.PhoneNumber)
                        {
                            Person = contact.DisplayName;
                            return;
                        }
                    }
                }
            }
        }

        public string MessageId { get; private set; }
        public string ThreadId { get; private set; }
        public string Address { get; private set; }
        public string Person { get; private set; }
        public Date Date { get; private set; }
        public string Text { get; private set; }
        public string Type { get; private set; }
    }
}