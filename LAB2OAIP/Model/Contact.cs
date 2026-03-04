using System;

/// <summary>
/// Summary description for Class1
/// </summary>
public class Contact
{
	public Contact()
	{

        public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public Contact()
    {
        Name = string.Empty;
        PhoneNumber = string.Empty;
        Email = string.Empty;
    }

    public Contact(string name, string phoneNumber, string email)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}
}
