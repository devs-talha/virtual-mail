using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Project.Models
{
    public class MailDAO
    {

        public bool InsertMail(Mail mail, int referenceMail)
        {
            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                NpgsqlCommand command = connection.CreateCommand();
                String commandText = "insert into public.mail (sender, receiver, subject, body, date) " +
                    "values (@senderId, @receiverId, " +
                    "@subject, @body, @date)";
                if (referenceMail != -1) 
                    commandText = "insert into public.mail (sender, receiver, subject, body, date, reference) " +
                    "values (@senderId, @receiverId, " +
                    "@subject, @body, @date, @reference)";
                command.CommandText = commandText;

                command.Parameters.AddWithValue("@senderId", mail.Sender.Id);
                command.Parameters.AddWithValue("@receiverId", mail.Receiver.Id);
                command.Parameters.AddWithValue("@subject", mail.Subject);
                command.Parameters.AddWithValue("@body", mail.Body);
                command.Parameters.AddWithValue("@date", DateTime.Now);
                if (referenceMail != -1)
                    command.Parameters.AddWithValue("@reference", referenceMail);

                return command.ExecuteNonQuery() > 0;
            }
        }

        public List<Mail> GetMail(int id, Boolean withReferences)
        {
            List<Mail> mails = new List<Mail>();
            GetMailHelper(id, mails, withReferences);
            return mails;
        }

        private void GetMailHelper(int id, List<Mail> mails, Boolean withReferences)
        {
            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                NpgsqlCommand command = connection.CreateCommand();
                command.CommandText = "select id, sender, receiver, body, subject, date::timestamp without time zone as \"date\", reference from mail where id = @id";
                command.Parameters.AddWithValue("@id", id);
                NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    UserDAO userDAO = new UserDAO();
                    Mail mail = new Mail(int.Parse(reader["id"].ToString()),
                        userDAO.GetUser(int.Parse(reader["sender"].ToString())),
                        userDAO.GetUser(int.Parse(reader["receiver"].ToString())),
                        reader["subject"].ToString(), reader["body"].ToString(),
                        DateTimeOffset.Parse(reader["date"].ToString()).DateTime);
                    mails.Insert(0, mail);
                    if (withReferences && reader["reference"].ToString() != "")
                    {
                        GetMailHelper(int.Parse(reader["reference"].ToString()), mails, withReferences);
                    }
                }
            }
        }


        public bool DeleteMail(int id)
        {
            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                NpgsqlCommand command = connection.CreateCommand();
                command.CommandText = "delete from public.mail " +
                    "where id = @id";
                command.Parameters.AddWithValue("@id", id);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public List<Mail> ListReceivedMails(User user)
        {
            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                NpgsqlCommand command = connection.CreateCommand();
                command.CommandText = "select m.id as \"mail_id\", m.subject as \"mail_subject\", m.body as \"mail_body\", " +
                    "m.date::timestamp without time zone as \"mail_date\", sender.id as \"sender_id\", sender.first_name as \"sender_first_name\", " +
                    "sender.last_name as \"sender_last_name\", sender.email as \"sender_email\", " +
                    "receiver.id as \"receiver_id\", receiver.first_name as \"receiver_first_name\", " +
                    "receiver.last_name as \"receiver_last_name\", receiver.email as \"receiver_email\" " +
                    "from public.mail m " +
                    "inner join " +
                    "public.user sender " +
                    "on m.sender = sender.id " +
                    "inner join " +
                    "public.user receiver " +
                    "on m.receiver = receiver.id " +
                    "where receiver.email = @email " +
                    "order by mail_date desc;";
                command.Parameters.AddWithValue("@email", user.Email);
                NpgsqlDataReader reader = command.ExecuteReader();
                List<Mail> mails = new List<Mail>();
                while (reader.Read())
                {
                    User sender = new User();
                    sender.Id = int.Parse(reader["sender_id"].ToString());
                    sender.FirstName = reader["sender_first_name"].ToString();
                    sender.LastName = reader["sender_last_name"].ToString();
                    sender.Email = reader["sender_email"].ToString();

                    User receiver = new User();
                    receiver.Id = int.Parse(reader["receiver_id"].ToString());
                    receiver.FirstName = reader["receiver_first_name"].ToString();
                    receiver.LastName = reader["receiver_last_name"].ToString();
                    receiver.Email = reader["receiver_email"].ToString();

                    Mail mail = new Mail(int.Parse(reader["mail_id"].ToString()),
                        sender, receiver, reader["mail_subject"].ToString(),
                        reader["mail_body"].ToString(), DateTimeOffset.Parse(reader["mail_date"].ToString()).DateTime);
                    mails.Add(mail);
                }
                return mails;
            }
        }

        public List<Mail> ListSentMails(User user)
        {
            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                NpgsqlCommand command = connection.CreateCommand();
                command.CommandText = "select m.id as \"mail_id\", m.subject as \"mail_subject\", m.body as \"mail_body\", " +
                    "m.date::timestamp without time zone as \"mail_date\", sender.id as \"sender_id\", sender.first_name as \"sender_first_name\", " +
                    "sender.last_name as \"sender_last_name\", sender.email as \"sender_email\", " +
                    "receiver.id as \"receiver_id\", receiver.first_name as \"receiver_first_name\", " +
                    "receiver.last_name as \"receiver_last_name\", receiver.email as \"receiver_email\" " +
                    "from public.mail m " +
                    "inner join " +
                    "public.user sender " +
                    "on m.sender = sender.id " +
                    "inner join " +
                    "public.user receiver " +
                    "on m.receiver = receiver.id " +
                    "where sender.email = @email " +
                    "order by mail_date desc;";
                command.Parameters.AddWithValue("@email", user.Email);
                NpgsqlDataReader reader = command.ExecuteReader();
                List<Mail> mails = new List<Mail>();
                while (reader.Read())
                {
                    User sender = new User();
                    sender.Id = int.Parse(reader["sender_id"].ToString());
                    sender.FirstName = reader["sender_first_name"].ToString();
                    sender.LastName = reader["sender_last_name"].ToString();
                    sender.Email = reader["sender_email"].ToString();

                    User receiver = new User();
                    receiver.Id = int.Parse(reader["receiver_id"].ToString());
                    receiver.FirstName = reader["receiver_first_name"].ToString();
                    receiver.LastName = reader["receiver_last_name"].ToString();
                    receiver.Email = reader["receiver_email"].ToString();

                    Mail mail = new Mail(int.Parse(reader["mail_id"].ToString()),
                        sender, receiver, reader["mail_subject"].ToString(),
                        reader["mail_body"].ToString(), DateTimeOffset.Parse(reader["mail_date"].ToString()).DateTime);
                    mails.Add(mail);
                }
                return mails;
            }
        }
    }
}
