using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (txtFIO.Text == "admin" && textBox1.Text == "12345")
                {
                    string connectionString = "Server=127.0.0.1;Database=data;Uid=root;Pwd=pbvf28101973;charset=utf8;";
                    MySqlConnection connection = new MySqlConnection(connectionString);
                    connection.Open();
                    MessageBox.Show("Вы успешно вошли в систему в качестве администратора!");
                    Form2 crudForm = new Form2(connection);

                    crudForm.Show();
                    this.Hide();
                    connection.Close();
                }
                else
                {
                    if (txtFIO.Text == "user" && textBox1.Text == "12345")
                    {
                        string connectionString = "Server=127.0.0.1;Database=data;Uid=root;Pwd=pbvf28101973;charset=utf8;";
                        MySqlConnection connection = new MySqlConnection(connectionString);
                        connection.Open();
                        var frm = new BillboardForm(connection);
                        MessageBox.Show("Вы успешно вошли в систему в качестве пользователя!");
                        frm.Show();
                        this.Hide();
                        connection.Close();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Неверный логин или пароль!");
            }
        }
    }
}
