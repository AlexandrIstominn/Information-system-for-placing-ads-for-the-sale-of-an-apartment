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
    public partial class InfoForm : Form
    {
        private MySqlConnection connection;
        private int billboardId;
        private int queryCode = -1;
        private int districtCode = -1;
        private int streetCode = -1;
        private int buildingCode = -1;
        private int flatCode = -1;
        private int typeCode = -1;
        public InfoForm(MySqlConnection connection, int billboardId)
        {
            InitializeComponent();
            this.connection = connection;
            this.billboardId = billboardId;
            FormLoad();
        }
        private void FormLoad()
        {
            string query = "SELECT Стоимость_Квартиры, Этажность_Дома, Этаж_Квартиры, Общая_Площадь, Адрес FROM доска_объявлений WHERE Код_Объявления = @billboardId";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@billboardId", billboardId);

            // Выполнение запроса и чтение данных
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    // Присваивание значений текстовым полям
                    txtCost.Text = reader.GetString(0); // Стоимость_Квартиры
                    txtNumFloors.Text = reader.GetString(1); // Этажность_Дома
                    txtFloor.Text = reader.GetString(2); // Этаж_Квартиры
                    txtArea.Text = reader.GetString(3); // Общая_Площадь

                    // Адрес состоит из двух частей - улицы и номера дома
                    string address = reader.GetString(4); // Адрес
                    string[] addressParts = address.Split(','); // Разделение адреса на части по запятой
                    if (addressParts.Length == 2)
                    {
                        txtStreet.Text = addressParts[0].Trim(); // Улица
                        txtNumHouse.Text = addressParts[1].Trim(); // Номер дома
                    }
                }
            }
            // Запрос к базе данных для получения информации о заявке
            string query2 = "SELECT Код_Заявки, Статус_Заявки, Дата_Открытия, Дата_Закрытия FROM заявки WHERE Код_Объявления = @billboardId";
            MySqlCommand cmd2 = new MySqlCommand(query2, connection);
            cmd2.Parameters.AddWithValue("@billboardId", billboardId);
            // Выполнение запроса и чтение данных
            using (MySqlDataReader reader2 = cmd2.ExecuteReader())
            {
                if (reader2.Read())
                {
                    int.TryParse(reader2.GetString(0), out queryCode);
                    txtStatus.Text = reader2.GetString(1); // Статус_Заявки
                    txtDateOpen.Text = reader2.GetDateTime(2).ToString("yyyy-MM-dd"); // Дата_Открытия
                    txtDateClose.Text = reader2.IsDBNull(3) ? "" : reader2.GetDateTime(3).ToString("yyyy-MM-dd"); // Дата_Закрытия (если значение NULL, то присваиваем пустую строку)
                }
            }
            // Запрос к базе данных для получения информации о клиенте
            string query3 = "SELECT Фамилия, Имя, Отчество, Контактный_Телефон, Дата_Рождения, Пол, Электронная_Почта FROM клиенты WHERE Код_Заявки = @queryCode";
            MySqlCommand cmd3 = new MySqlCommand(query3, connection);
            cmd3.Parameters.AddWithValue("@queryCode", queryCode);

            // Выполнение запроса и чтение данных
            using (MySqlDataReader reader3 = cmd3.ExecuteReader())
            {
                if (reader3.Read())
                {
                    // Присваивание значений текстовым полям, связанным с клиентом
                    txtClientSurname.Text = reader3.GetString(0); // Фамилия
                    txtClientName.Text = reader3.GetString(1); // Имя
                    txtClientOtches.Text = reader3.GetString(2); // Отчество
                    txtClientContactPhone.Text = reader3.GetString(3); // Контактный_Телефон
                    txtClientDateBirth.Text = reader3.GetDateTime(4).ToString("yyyy-MM-dd"); // Дата_Рождения
                    txtClientGender.Text = reader3.GetString(5); // Пол
                    txtClientMail.Text = reader3.GetString(6); // Электронная_Почта
                }
            }
            // районы
            string query1 = "SELECT Код_Района, Название_Района FROM районы WHERE Код_Объявления = @billboardId";
            MySqlCommand cmd1 = new MySqlCommand(query1, connection);
            cmd1.Parameters.AddWithValue("@billboardId", billboardId);

            // Выполнение запроса и чтение данных
            using (MySqlDataReader reader1 = cmd1.ExecuteReader())
            {
                if (reader1.Read())
                {
                    int.TryParse(reader1.GetString(0), out districtCode);
                    txtDistrict.Text = reader1.GetString(1); // Название_Района
                }
            }
            // улицы
            string query4 = "SELECT Код_Улицы, Название FROM Улицы WHERE Код_Района = @districtCode";
            MySqlCommand cmd4 = new MySqlCommand(query4, connection);
            cmd4.Parameters.AddWithValue("@districtCode", districtCode);

            // Выполнение запроса и чтение данных
            using (MySqlDataReader reader = cmd4.ExecuteReader())
            {
                if (reader.Read())
                {
                    int.TryParse(reader.GetString(0), out streetCode);
                    txtStreet.Text = reader.GetString(1); // Название улицы
                }
            }
            // здание
            string query5 = "SELECT Код_Здания, Номер_Дома, Этажность, Тип_Перекрытий, Количество_Лифтов, Типы_Лифтов, Наличие_Системы_Умный_Дом, Дата_Постройки FROM здание WHERE Код_Улицы = @streetCode";
            MySqlCommand cmd5 = new MySqlCommand(query5, connection);
            cmd5.Parameters.AddWithValue("@streetCode", streetCode);

            // Выполнение запроса и чтение данных
            using (MySqlDataReader reader = cmd5.ExecuteReader())
            {
                if (reader.Read())
                {
                    int.TryParse(reader.GetString(0), out buildingCode);
                    txtNumHouse.Text = reader.GetString(1); // Номер дома
                    txtNumFloors.Text = reader.GetString(2); // Этажность
                    txtTypeOverlaps.Text = reader.GetString(3); // Тип перекрытий
                    txtNumElevators.Text = reader.GetString(4); // Количество лифтов
                    txtTypeElevators.Text = reader.GetString(5); // Типы лифтов
                    txtSmartHome.Text = reader.GetString(6); // Наличие системы умный дом
                    txtDateConstruction.Text = reader.GetDateTime(7).ToString("yyyy-MM-dd"); // Дата постройки
                }
            }
            //квартиры
            string query6 = "SELECT Код_Квартиры, Этаж, Количество_Комнат, Общая_Площадь, Вид_Из_Окна, Площадь_Кухни, Наличие_Балкона_Лоджии FROM квартира WHERE Код_Здания = @buildingCode";
            MySqlCommand cmd6 = new MySqlCommand(query6, connection);
            cmd6.Parameters.AddWithValue("@buildingCode", buildingCode);

            // Выполнение запроса и чтение данных
            using (MySqlDataReader reader = cmd6.ExecuteReader())
            {
                if (reader.Read())
                {
                    int.TryParse(reader.GetString(0), out flatCode);
                    txtFloor.Text = reader.GetString(1); // Этаж
                    txtNumRooms.Text = reader.GetString(2); // Количество комнат
                    txtArea.Text = reader.GetString(3); // Общая площадь
                    txtView.Text = reader.GetString(4); // Вид из окна
                    txtKitchenArea.Text = reader.GetString(5); // Площадь кухни
                    txtBalcony.Text = reader.GetString(6); // Наличие балкона или лоджии
                }
            }
            //владелец_квартиры
            string query7 = "SELECT Фамилия, Имя, Отчество, Дата_Рождения, Пол, Электронная_Почта, Контактный_Телефон FROM владелец_квартиры WHERE Код_Квартиры = @flatCode";
            MySqlCommand cmd7 = new MySqlCommand(query7, connection);
            cmd7.Parameters.AddWithValue("@flatCode", flatCode); // Предположим, что apartmentId - это код выбранной квартиры

            // Выполнение запроса и чтение данных
            using (MySqlDataReader reader = cmd7.ExecuteReader())
            {
                if (reader.Read())
                {

                    txtOwnerSurname.Text = reader.GetString(0); // Фамилия
                    txtOwnerName.Text = reader.GetString(1); // Имя
                    txtOwnerOtches.Text = reader.GetString(2); // Отчество
                    txtOwnerDate.Text = reader.GetDateTime(3).ToString("yyyy-MM-dd"); // Дата рождения
                    txtOwnerGender.Text = reader.GetString(4); // Пол
                    txtOwnerMail.Text = reader.GetString(5); // Электронная почта
                    txtOwnerPhone.Text = reader.GetString(6); // Контактный телефон
                }
            }
            //тип_жилья
            string query8 = "SELECT Код_Типа, Тип_Жилья FROM тип_жилья WHERE Код_Здания = @buildingCode";
            MySqlCommand cmd8 = new MySqlCommand(query8, connection);
            cmd8.Parameters.AddWithValue("@buildingCode", buildingCode);

            // Выполнение запроса и чтение данных
            using (MySqlDataReader reader = cmd8.ExecuteReader())
            {
                if (reader.Read())
                {
                    int.TryParse(reader.GetString(0), out typeCode);
                    txtType.Text = reader.GetString(1); // Тип жилья
                }
            }
            //застройщик
            string query9 = "SELECT Название, ИНН, Рейтинг_ЕРЗ, Квартир_В_Продаже, Телефон FROM застройщик WHERE Код_Типа = @typeCode";
            MySqlCommand cmd9 = new MySqlCommand(query9, connection);
            cmd9.Parameters.AddWithValue("@typeCode", typeCode);

            // Выполнение запроса и чтение данных
            using (MySqlDataReader reader = cmd9.ExecuteReader())
            {
                if (reader.Read())
                {
                    txtName.Text = reader.GetString(0); // Название
                    txtINN.Text = reader.GetString(1); // ИНН
                    txtERZ.Text = reader.GetString(2); // Рейтинг ЕРЗ
                    txtNumSell.Text = reader.GetString(3); // Квартир в продаже
                    txtPhone.Text = reader.GetString(4); // Телефон
                }
            }
        }
        private void btnCancelMain_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOKMain_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
