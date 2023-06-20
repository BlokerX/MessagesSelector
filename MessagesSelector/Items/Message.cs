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
                        if (PhoneNumberConverter(address) == PhoneNumberConverter(phone.PhoneNumber))
                        {
                            Person = contact.DisplayName;
                            return;
                        }
                    }
                }
            }
        }

        public static string PhoneNumberConverter(string number)
        {
            if (number == null)
                return null;

            string tmp = string.Empty;
            int i = 0;

            if (number.StartsWith('+'))
            {
                i = 3;
            }

            for (; i < number.Length; i++)
            {
                if (number[i] != ' ')
                    tmp += number[i];
            }

            return tmp;
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