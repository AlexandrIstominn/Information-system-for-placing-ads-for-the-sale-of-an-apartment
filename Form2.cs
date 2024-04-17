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
using MySqlX.XDevAPI;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        private MySqlConnection connection;
        public Form2(MySqlConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                string query = "SELECT CONCAT(клиенты.Фамилия, ' ', клиенты.Имя, ' ', клиенты.Отчество) AS ФИО, клиенты.Контактный_Телефон, заявки.Статус_Заявки, заявки.Дата_Открытия, заявки.Дата_Закрытия FROM клиенты JOIN заявки ON клиенты.Код_Заявки = заявки.Код_Заявки";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка загрузки данных!");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0) // Проверяем, что есть выделенные ячейки
            {
                int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex; // Получаем индекс выделенной строки
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex]; // Получаем объект выделенной строки

                string phoneNumber = selectedRow.Cells["Контактный_Телефон"].Value.ToString(); // Получаем значение контактного телефона из выделенной строки
                int requestId = GetRequestIdByPhoneNumber(phoneNumber); // Получаем код заявки по контактному телефону

                if (requestId != -1) // Если найден код заявки
                {
                    int billboardId = GetBillboardIdByRequestId(requestId); // Получаем код объявления по коду заявки

                    if (billboardId != -1) // Если найден код объявления
                    {
                        DeleteBillboardByBillboardId(billboardId); // Удаляем объявление по коду объявления
                        MessageBox.Show("Клиент успешно удалён!");
                        LoadData(); // Обновляем данные в DataGridView
                    }
                    else
                    {
                        MessageBox.Show("Не найдено объявление с кодом заявки " + requestId);
                    }
                }
                else
                {
                    MessageBox.Show("Не найдена заявка для контактного телефона " + phoneNumber);
                }
            }
            else
            {
                MessageBox.Show("Не выбрана строка для удаления объявления.");
            }
        }
        // Метод для получения кода объявления по коду заявки
        private int GetBillboardIdByRequestId(int requestId)
        {
            int billboardId = -1;
            try
            {
                string query = "SELECT Код_Объявления FROM Заявки WHERE Код_Заявки = @requestId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@requestId", requestId);

                connection.Open();
                billboardId = Convert.ToInt32(cmd.ExecuteScalar());
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении кода объявления: " + ex.Message);
            }
            return billboardId;
        }
        private int GetRequestIdByPhoneNumber(string phoneNumber)
        {
            try
            {
                string query = "SELECT заявки.Код_Заявки FROM заявки JOIN клиенты ON заявки.Код_Заявки = клиенты.Код_Заявки WHERE клиенты.Контактный_Телефон = @phoneNumber";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                connection.Open();
                object result = cmd.ExecuteScalar();
                connection.Close();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    return -1; // Возвращаем -1 в случае, если код заявки не найден
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске кода заявки: " + ex.Message);
                return -1; // Возвращаем -1 в случае возникновения ошибки
            }
        }

        // Метод для удаления объявления по коду объявления
        private void DeleteBillboardByBillboardId(int billboardId)
        {
            try
            {
                string query = "DELETE FROM Доска_Объявлений WHERE Код_Объявления = @billboardId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@billboardId", billboardId);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении объявления: " + ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=127.0.0.1;Database=data;Uid=root;Pwd=pbvf28101973;charset=utf8;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            var frm = new AddClientForm(connection);
            frm.ShowDialog();
            LoadData();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        
        private void button3_Click_1(object sender, EventArgs e)
        {
            
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnBillboard_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=127.0.0.1;Database=data;Uid=root;Pwd=pbvf28101973;charset=utf8;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            var frm = new BillboardForm(connection);
            frm.ShowDialog();
            LoadData();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0) // Проверяем, что есть выделенные ячейки
            {
                int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex; // Получаем индекс выделенной строки
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex]; // Получаем объект выделенной строки

                string phoneNumber = selectedRow.Cells["Контактный_Телефон"].Value.ToString(); // Получаем значение контактного телефона из выделенной строки
                int requestId = GetRequestIdByPhoneNumber(phoneNumber); // Получаем код заявки по контактному телефону

                if (requestId != -1) // Если найден код заявки
                {
                    int billboardId = GetBillboardIdByRequestId(requestId); // Получаем код объявления по коду заявки

                    if (billboardId != -1) // Если найден код объявления
                    {
                        string connectionString = "Server=127.0.0.1;Database=data;Uid=root;Pwd=pbvf28101973;charset=utf8;";
                        MySqlConnection connection = new MySqlConnection(connectionString);
                        connection.Open();
                        var frm = new EditClientForm(connection, billboardId);
                        frm.ShowDialog();
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Не найдено объявление с кодом заявки " + requestId);
                    }
                }
                else
                {
                    MessageBox.Show("Не найдена заявка для контактного телефона " + phoneNumber);
                }
            }
            else
            {
                MessageBox.Show("Не выбрана строка для удаления объявления.");
            }
        }
    }
}
