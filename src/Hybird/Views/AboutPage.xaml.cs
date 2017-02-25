using System;
using Plugin.Messaging;

namespace Hybird.Views
{
	public partial class AboutPage
	{
		public AboutPage()
		{
			InitializeComponent();
			contactAuthorButton.IsVisible =
				CrossMessaging.Current.EmailMessenger.CanSendEmail
				|| CrossMessaging.Current.SmsMessenger.CanSendSms;
		}

		void ContactAuthor(object sender, EventArgs e)
		{
			var emailMessenger = CrossMessaging.Current.EmailMessenger;
			if (emailMessenger.CanSendEmail)
			{
				emailMessenger.SendEmail("imgen@hotmail.com");
			}
			else
			{
				var smsMessenger = CrossMessaging.Current.SmsMessenger;
				if (smsMessenger.CanSendSms)
				{
					smsMessenger.SendSms("+8618823214892");
				}
			}
		}
	}
}
