using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class EditClientForm : Form
    {
        private MySqlConnection connection;
        private int billboardId;
        private int queryCode = -1;
        private int districtCode = -1;
        private int streetCode = -1;
        private int buildingCode = -1;
        private int flatCode = -1;
        private int typeCode = -1;
        public EditClientForm(MySqlConnection connection, int billboardId)
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

        private void btnOKMain_Click(object sender, EventArgs e)
        {
            errorProviderAddClient.Clear();
            //фамилия
            Regex regex = new Regex(@"^[А-Яа-я\s]+$");
            if (string.IsNullOrWhiteSpace(txtClientName.Text))
            {
                errorProviderAddClient.SetError(txtClientName, "Значение поля не может быть пустым!");
            }
            else
            {
                if (!regex.IsMatch(txtClientName.Text))
                {
                    errorProviderAddClient.SetError(txtClientName, "Поле должно содержать только русские буквы и пробелы!");
                }
            }
            //имя
            regex = new Regex(@"^[А-Яа-я\s]+$");
            if (string.IsNullOrWhiteSpace(txtClientSurname.Text))
            {
                errorProviderAddClient.SetError(txtClientSurname, "Значение поля не может быть пустым!");
            }
            else
            {
                if (!regex.IsMatch(txtClientName.Text))
                {
                    errorProviderAddClient.SetError(txtClientSurname, "Поле должно содержать только русские буквы и пробелы!");
                }
            }
            //отчество
            regex = new Regex(@"^[А-Яа-я\s]+$");
            if (string.IsNullOrWhiteSpace(txtClientOtches.Text))
            {
                errorProviderAddClient.SetError(txtClientOtches, "Значение поля не может быть пустым!");
            }
            else
            {
                if (!regex.IsMatch(txtClientOtches.Text))
                {
                    errorProviderAddClient.SetError(txtClientOtches, "Поле должно содержать только русские буквы и пробелы!");
                }
            }
            //контактный телефон
            if (string.IsNullOrWhiteSpace(txtClientContactPhone.Text))
            {
                errorProviderAddClient.SetError(txtClientContactPhone, "Значение поля не может быть пустым!");
            }
            else
            {
                if (!long.TryParse(txtClientContactPhone.Text, out _) || txtClientContactPhone.Text.Length != 11)
                {
                    errorProviderAddClient.SetError(txtClientContactPhone, "Значение поля должно быть числовым и иметь длину 11 символов!");
                }
                else if (txtClientContactPhone.Text[0] != '8')
                {
                    errorProviderAddClient.SetError(txtClientContactPhone, "Значение поля должно начинаться с '8'!");
                }
            }
            //дата рождения
            DateTime dateOfBirth;
            if (string.IsNullOrWhiteSpace(txtClientDateBirth.Text))
            {
                errorProviderAddClient.SetError(txtClientDateBirth, "Значение поля не может быть пустым!");
            }
            else
            {
                if (!DateTime.TryParseExact(txtClientDateBirth.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth))
                {
                    errorProviderAddClient.SetError(txtClientDateBirth, "Некорректный формат даты! Используйте формат ГГГГ-ММ-ДД");
                }
                else if (dateOfBirth > DateTime.Now)
                {
                    errorProviderAddClient.SetError(txtClientDateBirth, "Дата рождения не может быть в будущем!");
                }
                else
                {
                    // Поле заполнено корректно, очищаем ошибку
                    errorProviderAddClient.SetError(txtClientDateBirth, "");
                }
            }
            //пол
            if (string.IsNullOrWhiteSpace(txtClientGender.Text))
            {
                errorProviderAddClient.SetError(txtClientGender, "Значение поля не может быть пустым!");
            }
            else
            {
                if (!regex.IsMatch(txtClientGender.Text))
                {
                    errorProviderAddClient.SetError(txtClientGender, "Поле должно содержать только русские буквы и пробелы!");
                }
            }
            //электронная почта
            if (string.IsNullOrWhiteSpace(txtClientMail.Text))
            {
                errorProviderAddClient.SetError(txtClientMail, "Значение поля не может быть пустым!");
            }
            else
            {
                string email = txtClientMail.Text.Trim();
                if (!IsValidMail(email))
                {
                    errorProviderAddClient.SetError(txtClientMail, "Неправильный формат электронной почты!");
                }
                else
                {
                    errorProviderAddClient.SetError(txtClientMail, "");
                }
            }
            bool IsValidMail(string email)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == email;
                }
                catch
                {
                    return false;
                }
            }
            //статус заявки
            if (string.IsNullOrWhiteSpace(txtStatus.Text))
            {
                errorProviderAddClient.SetError(txtStatus, "Значение поля не может быть пустым!");
            }
            else
            {
                errorProviderAddClient.SetError(txtStatus, "");
            }
            //дата открытия
            DateTime dateOfBirth3;
            if (string.IsNullOrWhiteSpace(txtDateOpen.Text))
            {
                errorProviderAddClient.SetError(txtDateOpen, "Значение поля не может быть пустым!");
            }
            else
            {
                if (!DateTime.TryParseExact(txtDateOpen.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth3))
                {
                    errorProviderAddClient.SetError(txtDateOpen, "Некорректный формат даты! Используйте формат ГГГГ-ММ-ДД");
                }
                else if (dateOfBirth3 > DateTime.Now)
                {
                    errorProviderAddClient.SetError(txtDateOpen, "Дата открытия не может быть в будущем!");
                }
                else
                {
                    errorProviderAddClient.SetError(txtDateOpen, "");
                }
            }
            //дата закрытия
            DateTime dateOfBirth1;
            if (string.IsNullOrWhiteSpace(txtDateClose.Text))
            {
            }
            else
            {
                if (!DateTime.TryParseExact(txtDateClose.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth1))
                {
                    errorProviderAddClient.SetError(txtDateClose, "Некорректный формат даты! Используйте формат ГГГГ-ММ-ДД");
                }
                else if (dateOfBirth1 > DateTime.Now)
                {
                    errorProviderAddClient.SetError(txtDateClose, "Дата открытия не может быть в будущем!");
                }
                else
                {
                    errorProviderAddClient.SetError(txtDateClose, "");
                }
            }
            //название района
            if (string.IsNullOrWhiteSpace(txtDistrict.Text))
            {
                errorProviderAddClient.SetError(txtDistrict, "Значение поля не может быть пустым!");
            }
            else
            {
                errorProviderAddClient.SetError(txtDistrict, "");
            }
            //название улицы
            if (string.IsNullOrWhiteSpace(txtStreet.Text))
            {
                errorProviderAddClient.SetError(txtStreet, "Значение поля не может быть пустым!");
            }
            else
            {
                errorProviderAddClient.SetError(txtStreet, "");
            }
            //тип жилья
            if (string.IsNullOrWhiteSpace(txtType.Text))
            {
                errorProviderAddClient.SetError(txtType, "Значение поля не может быть пустым!");
            }
            else
            {
                errorProviderAddClient.SetError(txtType, "");
            }
            //номер дома
            if (string.IsNullOrWhiteSpace(txtNumHouse.Text))
            {
                errorProviderAddClient.SetError(txtNumHouse, "Значение поля не может быть пустым!");
            }
            else
            {
                errorProviderAddClient.SetError(txtNumHouse, "");
            }
            //этажность
            if (string.IsNullOrWhiteSpace(txtNumFloors.Text))
            {
                errorProviderAddClient.SetError(txtNumFloors, "Значение поля не может быть пустым!");
            }
            else
            {
                if (!int.TryParse(txtNumFloors.Text, out _))
                {
                    errorProviderAddClient.SetError(txtNumFloors, "Значение поля должно быть числовым!");
                }
                else
                {
                    errorProviderAddClient.SetError(txtNumFloors, "");
                }
            }
            //тип перекрытий
            if (string.IsNullOrWhiteSpace(txtTypeOverlaps.Text))
            {
                errorProviderAddClient.SetError(txtTypeOverlaps, "Значение поля не может быть пустым!");
            }
            else
            {
                errorProviderAddClient.SetError(txtTypeOverlaps, ""); // Очищаем ошибку, если поле заполнено
            }
            //количество лифтов
            if (string.IsNullOrWhiteSpace(txtNumElevators.Text))
            {
                errorProviderAddClient.SetError(txtNumElevators, "Значение поля не может быть пустым!");
            }
            else
            {
                if (!int.TryParse(txtNumElevators.Text, out _))
                {
                    errorProviderAddClient.SetError(txtNumElevators, "Значение поля должно быть числовым!");
                }
                else
                {
                    errorProviderAddClient.SetError(txtNumElevators, "");
                }
            }
            //типы лифтов
            if (string.IsNullOrWhiteSpace(txtTypeElevators.Text))
            {
                errorProviderAddClient.SetError(txtTypeElevators, "Значение поля не может быть пустым!");
            }
            else
            {
                errorProviderAddClient.SetError(txtTypeElevators, "");
            }
            //наличие системы умный дом
            if (string.IsNullOrWhiteSpace(txtSmartHome.Text) || (txtSmartHome.Text != "0" && txtSmartHome.Text != "1"))
            {
                errorProviderAddClient.SetError(txtSmartHome, "Поле должно содержать значение 0 при отрицательном ответе или 1 при положительном!");
            }
            else
            {
                errorProviderAddClient.SetError(txtSmartHome, "");
            }
            //дата постройки
            DateTime dateOfBirth2;
            if (string.IsNullOrWhiteSpace(txtDateConstruction.Text))
            {
                errorProviderAddClient.SetError(txtDateConstruction, "Значение поля не может быть пустым!");
            }
            else
            {
                if (!DateTime.TryParseExact(txtDateConstruction.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth2))
                {
                    errorProviderAddClient.SetError(txtDateConstruction, "Некорректный формат даты! Используйте формат ГГГГ-ММ-ДД");
                }
                else if (dateOfBirth2 > DateTime.Now)
                {
                    errorProviderAddClient.SetError(txtDateConstruction, "Дата постройки не может быть в будущем!");
                }
                else
                {
                    errorProviderAddClient.SetError(txtDateConstruction, "");
                }
            }
            //этаж
            if (string.IsNullOrWhiteSpace(txtFloor.Text))
            {
                errorProviderAddClient.SetError(txtFloor, "Поле не может быть пустым!");
            }
            else if (!int.TryParse(txtFloor.Text, out _))
            {
                errorProviderAddClient.SetError(txtFloor, "Этаж должен быть числом!");
            }
            else
            {
                errorProviderAddClient.SetError(txtFloor, "");
            }

            //количество комнат
            if (string.IsNullOrWhiteSpace(txtNumRooms.Text))
            {
                errorProviderAddClient.SetError(txtNumRooms, "Поле не может быть пустым!");
            }
            else if (!int.TryParse(txtNumRooms.Text, out _))
            {
                errorProviderAddClient.SetError(txtNumRooms, "Количество комнат должно быть числом!");
            }
            else
            {
                errorProviderAddClient.SetError(txtNumRooms, "");
            }

            //общая площадь
            if (string.IsNullOrWhiteSpace(txtArea.Text))
            {
                errorProviderAddClient.SetError(txtArea, "Поле не может быть пустым!");
            }
            else if (!double.TryParse(txtArea.Text, out _))
            {
                errorProviderAddClient.SetError(txtArea, "Общая площадь должна быть числом!");
            }
            else
            {
                errorProviderAddClient.SetError(txtArea, "");
            }

            //площадь кухни
            if (string.IsNullOrWhiteSpace(txtKitchenArea.Text))
            {
                errorProviderAddClient.SetError(txtKitchenArea, "Поле не может быть пустым!");
            }
            else if (!double.TryParse(txtKitchenArea.Text, out _))
            {
                errorProviderAddClient.SetError(txtKitchenArea, "Площадь кухни должна быть числом!");
            }
            else
            {
                errorProviderAddClient.SetError(txtKitchenArea, "");
            }
            //вид из окна
            if (string.IsNullOrWhiteSpace(txtView.Text))
            {
                errorProviderAddClient.SetError(txtView, "Поле не может быть пустым!");
            }
            else
            {
                errorProviderAddClient.SetError(txtView, "");
            }

            //наличие балкона или лоджии
            if (string.IsNullOrWhiteSpace(txtBalcony.Text))
            {
                errorProviderAddClient.SetError(txtBalcony, "Поле не может быть пустым!");
            }
            else if (txtBalcony.Text != "0" && txtBalcony.Text != "1")
            {
                errorProviderAddClient.SetError(txtBalcony, "Значение должно быть 0 при отрицательном ответе или 1 при положительном!");
            }
            else
            {
                errorProviderAddClient.SetError(txtBalcony, "");
            }

            //стоимость
            if (string.IsNullOrWhiteSpace(txtCost.Text))
            {
                errorProviderAddClient.SetError(txtCost, "Поле не может быть пустым!");
            }
            else if (!double.TryParse(txtCost.Text, out _))
            {
                errorProviderAddClient.SetError(txtCost, "Стоимость должна быть числом!");
            }
            else
            {
                errorProviderAddClient.SetError(txtCost, "");
            }
            //фамилия владельца
            if (string.IsNullOrWhiteSpace(txtOwnerName.Text))
            {
                errorProviderAddClient.SetError(txtOwnerName, "Поле не может быть пустым!");
            }
            else
            {
                errorProviderAddClient.SetError(txtOwnerName, "");
            }

            //имя владельца
            if (string.IsNullOrWhiteSpace(txtOwnerSurname.Text))
            {
                errorProviderAddClient.SetError(txtOwnerSurname, "Поле не может быть пустым!");
            }
            else
            {
                errorProviderAddClient.SetError(txtOwnerSurname, "");
            }

            //отчество владельца
            if (string.IsNullOrWhiteSpace(txtOwnerOtches.Text))
            {
                errorProviderAddClient.SetError(txtOwnerOtches, "Поле не может быть пустым!");
            }
            else
            {
                errorProviderAddClient.SetError(txtOwnerOtches, "");
            }

            //контактный телефон владельца
            if (string.IsNullOrWhiteSpace(txtOwnerPhone.Text))
            {
                errorProviderAddClient.SetError(txtOwnerPhone, "Поле не может быть пустым!");
            }
            else if (txtOwnerPhone.Text.Length != 11 || !txtOwnerPhone.Text.StartsWith("8") || !txtOwnerPhone.Text.All(char.IsDigit))
            {
                errorProviderAddClient.SetError(txtOwnerPhone, "Неверный формат телефона! Должно быть 11 цифр и начинаться с '8'.");
            }
            else
            {
                errorProviderAddClient.SetError(txtOwnerPhone, "");
            }
            //дата рождения владельца
            if (string.IsNullOrWhiteSpace(txtOwnerDate.Text))
            {
                errorProviderAddClient.SetError(txtOwnerDate, "Поле не может быть пустым!");
            }
            else if (!DateTime.TryParseExact(txtOwnerDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                errorProviderAddClient.SetError(txtOwnerDate, "Неверный формат даты! Должен быть в формате 'год-месяц-день'.");
            }
            else
            {
                errorProviderAddClient.SetError(txtOwnerDate, "");
            }

            //пол владельца
            if (string.IsNullOrWhiteSpace(txtOwnerGender.Text))
            {
                errorProviderAddClient.SetError(txtOwnerGender, "Поле не может быть пустым!");
            }
            else
            {
                errorProviderAddClient.SetError(txtOwnerGender, "");
            }

            //электронная почта владельца
            if (string.IsNullOrWhiteSpace(txtOwnerMail.Text))
            {
                errorProviderAddClient.SetError(txtOwnerMail, "Поле не может быть пустым!");
            }
            else if (!Regex.IsMatch(txtOwnerMail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errorProviderAddClient.SetError(txtOwnerMail, "Неверный формат электронной почты!");
            }
            else
            {
                errorProviderAddClient.SetError(txtOwnerMail, "");
            }
            //название
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errorProviderAddClient.SetError(txtName, "Поле не может быть пустым!");
            }
            else
            {
                errorProviderAddClient.SetError(txtName, "");
            }

            //ИНН
            if (string.IsNullOrWhiteSpace(txtINN.Text))
            {
                errorProviderAddClient.SetError(txtINN, "Поле не может быть пустым!");
            }
            else if (!Regex.IsMatch(txtINN.Text, @"^\d{10}$"))
            {
                errorProviderAddClient.SetError(txtINN, "Неверный формат ИНН! ИНН должен состоять из 10 цифр.");
            }
            else
            {
                errorProviderAddClient.SetError(txtINN, "");
            }

            //рейтинг ЕРЗ
            if (string.IsNullOrWhiteSpace(txtERZ.Text))
            {
                errorProviderAddClient.SetError(txtERZ, "Поле не может быть пустым!");
            }
            else if (!int.TryParse(txtERZ.Text, out int erzRating) || erzRating < 0)
            {
                errorProviderAddClient.SetError(txtERZ, "Неверное значение! Рейтинг ЕРЗ должен быть неотрицательным целым числом.");
            }
            else
            {
                errorProviderAddClient.SetError(txtERZ, "");
            }

            //квартир в продаже
            if (string.IsNullOrWhiteSpace(txtNumSell.Text))
            {
                errorProviderAddClient.SetError(txtNumSell, "Поле не может быть пустым!");
            }
            else if (!int.TryParse(txtNumSell.Text, out int numSell) || numSell < 0)
            {
                errorProviderAddClient.SetError(txtNumSell, "Неверное значение! Количество квартир в продаже должно быть неотрицательным целым числом.");
            }
            else
            {
                errorProviderAddClient.SetError(txtNumSell, "");
            }

            //телефон
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                errorProviderAddClient.SetError(txtPhone, "Поле не может быть пустым!");
            }
            else if (!Regex.IsMatch(txtPhone.Text, @"^8\d{10}$"))
            {
                errorProviderAddClient.SetError(txtPhone, "Неверный формат номера телефона! Номер должен начинаться с '8' и содержать 11 цифр.");
            }
            else
            {
                errorProviderAddClient.SetError(txtPhone, "");
            }
            if (string.IsNullOrEmpty(errorProviderAddClient.GetError(txtName)) &&  //
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtINN)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtNumSell)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtERZ)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtPhone)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtFloor)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtNumRooms)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtArea)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtKitchenArea)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtView)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtBalcony)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtCost)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtOwnerSurname)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtOwnerName)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtOwnerOtches)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtOwnerDate)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtOwnerGender)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtOwnerMail)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtOwnerPhone)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtDistrict)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtStreet)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtType)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtNumHouse)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtNumFloors)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtTypeOverlaps)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtNumElevators)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtTypeElevators)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtSmartHome)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtDateConstruction)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtClientSurname)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtClientName)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtClientOtches)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtClientContactPhone)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtClientDateBirth)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtClientGender)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtClientMail)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtStatus)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtDateOpen)) &&//
                string.IsNullOrEmpty(errorProviderAddClient.GetError(txtDateClose))//
                )
            {
                // Обновление таблицы доска_объявлений
                string address = $"{txtStreet.Text}, {txtNumHouse.Text}";
                string query1 = "UPDATE доска_объявлений SET Стоимость_Квартиры = @Стоимость_Квартиры, Этажность_Дома = @Этажность_Дома, Этаж_Квартиры = @Этаж_Квартиры, Общая_Площадь = @Общая_Площадь, Адрес = @Адрес WHERE Код_Объявления = @billboardId";
                MySqlCommand cmd1 = new MySqlCommand(query1, connection);
                cmd1.Parameters.AddWithValue("@Стоимость_Квартиры", txtCost.Text);
                cmd1.Parameters.AddWithValue("@Этажность_Дома", txtNumFloors.Text);
                cmd1.Parameters.AddWithValue("@Этаж_Квартиры", txtFloor.Text);
                cmd1.Parameters.AddWithValue("@Общая_Площадь", txtArea.Text);
                cmd1.Parameters.AddWithValue("@Адрес", address);
                cmd1.Parameters.AddWithValue("@billboardId", billboardId);
                cmd1.ExecuteNonQuery();

                // Обновление таблицы заявки
                string query2 = "UPDATE заявки SET Статус_Заявки = @Статус_Заявки, Дата_Открытия = @Дата_Открытия, Дата_Закрытия = @Дата_Закрытия WHERE Код_Заявки = @queryCode";
                MySqlCommand cmd2 = new MySqlCommand(query2, connection);
                cmd2.Parameters.AddWithValue("@Статус_Заявки", txtStatus.Text);
                cmd2.Parameters.AddWithValue("@Дата_Открытия", txtDateOpen.Text);
                if (!string.IsNullOrWhiteSpace(txtDateClose.Text))
                {
                    cmd2.Parameters.AddWithValue("@Дата_Закрытия", txtDateClose.Text);
                }
                else
                {
                    cmd2.Parameters.AddWithValue("@Дата_Закрытия", DBNull.Value);
                }
                cmd2.Parameters.AddWithValue("@Код_Объявления", billboardId);
                cmd2.Parameters.AddWithValue("@queryCode", queryCode);
                cmd2.ExecuteNonQuery();

                // Обновление таблицы клиенты
                string query3 = "UPDATE клиенты SET Фамилия = @Фамилия, Имя = @Имя, Отчество = @Отчество, Контактный_Телефон = @Контактный_Телефон, Дата_Рождения = @Дата_Рождения, Пол = @Пол, Электронная_Почта = @Электронная_Почта WHERE Код_Заявки = @Код_Заявки";
                MySqlCommand cmd3 = new MySqlCommand(query3, connection);
                cmd3.Parameters.AddWithValue("@Фамилия", txtClientSurname.Text);
                cmd3.Parameters.AddWithValue("@Имя", txtClientName.Text);
                cmd3.Parameters.AddWithValue("@Отчество", txtClientOtches.Text);
                cmd3.Parameters.AddWithValue("@Контактный_Телефон", txtClientContactPhone.Text);
                cmd3.Parameters.AddWithValue("@Дата_Рождения", txtClientDateBirth.Text);
                cmd3.Parameters.AddWithValue("@Пол", txtClientGender.Text);
                cmd3.Parameters.AddWithValue("@Электронная_Почта", txtClientMail.Text);
                cmd3.Parameters.AddWithValue("@Код_Заявки", queryCode);
                cmd3.ExecuteNonQuery();

                // Обновление таблицы районы
                string query4 = "UPDATE районы SET Название_Района = @Название_Района WHERE Код_Объявления = @Код_Объявления";
                MySqlCommand cmd4 = new MySqlCommand(query4, connection);
                cmd4.Parameters.AddWithValue("@Название_Района", txtDistrict.Text);
                cmd4.Parameters.AddWithValue("@Код_Объявления", billboardId);
                cmd4.ExecuteNonQuery();

                // Обновление таблицы Улицы
                string query5 = "UPDATE Улицы SET Название = @Название WHERE Код_Района = @Код_Района";
                MySqlCommand cmd5 = new MySqlCommand(query5, connection);
                cmd5.Parameters.AddWithValue("@Название", txtStreet.Text);
                cmd5.Parameters.AddWithValue("@Код_Района", districtCode); // Подставьте соответствующий код района
                cmd5.ExecuteNonQuery();

                // Обновление таблицы здание
                string query6 = "UPDATE здание SET Номер_Дома = @Номер_Дома, Этажность = @Этажность, Тип_Перекрытий = @Тип_Перекрытий, Количество_Лифтов = @Количество_Лифтов, Типы_Лифтов = @Типы_Лифтов, Наличие_Системы_Умный_Дом = @Наличие_Системы_Умный_Дом, Дата_Постройки = @Дата_Постройки WHERE Код_Улицы = @Код_Улицы";
                MySqlCommand cmd6 = new MySqlCommand(query6, connection);
                cmd6.Parameters.AddWithValue("@Номер_Дома", txtNumHouse.Text);
                cmd6.Parameters.AddWithValue("@Этажность", txtNumFloors.Text);
                cmd6.Parameters.AddWithValue("@Тип_Перекрытий", txtTypeOverlaps.Text);
                cmd6.Parameters.AddWithValue("@Количество_Лифтов", txtNumElevators.Text);
                cmd6.Parameters.AddWithValue("@Типы_Лифтов", txtTypeElevators.Text);
                cmd6.Parameters.AddWithValue("@Наличие_Системы_Умный_Дом", txtSmartHome.Text);
                cmd6.Parameters.AddWithValue("@Дата_Постройки", txtDateConstruction.Text);
                cmd6.Parameters.AddWithValue("@Код_Улицы", streetCode); // Подставьте соответствующий код улицы
                cmd6.ExecuteNonQuery();

                //тип_жилья
                string query8 = "UPDATE тип_жилья SET Тип_Жилья = @Тип_Жилья WHERE Код_Здания = @Код_Здания";
                MySqlCommand cmd8 = new MySqlCommand(query8, connection);
                cmd8.Parameters.AddWithValue("@Тип_Жилья", txtType.Text);
                cmd8.Parameters.AddWithValue("@Код_Здания", buildingCode);
                cmd8.ExecuteNonQuery();

                //застройщик
                string query10 = "UPDATE застройщик SET Название = @Название, ИНН = @ИНН, Рейтинг_ЕРЗ = @Рейтинг_ЕРЗ, Квартир_В_Продаже = @Квартир_В_Продаже, Телефон = @Телефон WHERE Код_Типа = @Код_Типа";
                MySqlCommand cmd10 = new MySqlCommand(query10, connection);
                cmd10.Parameters.AddWithValue("@Название", txtName.Text);
                cmd10.Parameters.AddWithValue("@ИНН", txtINN.Text);
                cmd10.Parameters.AddWithValue("@Рейтинг_ЕРЗ", txtERZ.Text);
                cmd10.Parameters.AddWithValue("@Квартир_В_Продаже", txtNumSell.Text);
                cmd10.Parameters.AddWithValue("@Телефон", txtPhone.Text);
                cmd10.Parameters.AddWithValue("@Код_Типа", typeCode);
                cmd10.ExecuteNonQuery();

                //квартира
                string query7 = "UPDATE квартира SET Этаж = @Этаж, Количество_Комнат = @Количество_Комнат, Общая_Площадь = @Общая_Площадь, Вид_Из_Окна = @Вид_Из_Окна, Площадь_Кухни = @Площадь_Кухни, Наличие_Балкона_Лоджии = @Наличие_Балкона_Лоджии WHERE Код_Здания = @Код_Здания";
                MySqlCommand cmd7 = new MySqlCommand(query7, connection);
                cmd7.Parameters.AddWithValue("@Этаж", txtFloor.Text);
                cmd7.Parameters.AddWithValue("@Количество_Комнат", txtNumRooms.Text);
                cmd7.Parameters.AddWithValue("@Общая_Площадь", txtArea.Text);
                cmd7.Parameters.AddWithValue("@Вид_Из_Окна", txtView.Text);
                cmd7.Parameters.AddWithValue("@Площадь_Кухни", txtKitchenArea.Text);
                cmd7.Parameters.AddWithValue("@Наличие_Балкона_Лоджии", txtBalcony.Text);
                cmd7.Parameters.AddWithValue("@Код_Здания", buildingCode);
                cmd7.ExecuteNonQuery();

                //владелец_квартиры
                string query9 = "UPDATE владелец_квартиры SET Фамилия = @Фамилия, Имя = @Имя, Отчество = @Отчество, Дата_Рождения = @Дата_Рождения, Пол = @Пол, Электронная_Почта = @Электронная_Почта, Контактный_Телефон = @Контактный_Телефон WHERE Код_Квартиры = @Код_Квартиры";
                MySqlCommand cmd9 = new MySqlCommand(query9, connection);
                cmd9.Parameters.AddWithValue("@Фамилия", txtOwnerName.Text);
                cmd9.Parameters.AddWithValue("@Имя", txtOwnerSurname.Text);
                cmd9.Parameters.AddWithValue("@Отчество", txtOwnerOtches.Text);
                cmd9.Parameters.AddWithValue("@Дата_Рождения", txtOwnerDate.Text);
                cmd9.Parameters.AddWithValue("@Пол", txtOwnerGender.Text);
                cmd9.Parameters.AddWithValue("@Электронная_Почта", txtOwnerMail.Text);
                cmd9.Parameters.AddWithValue("@Контактный_Телефон", txtOwnerPhone.Text);
                cmd9.Parameters.AddWithValue("@Код_Квартиры", flatCode);
                cmd9.ExecuteNonQuery();


                connection.Close();
                MessageBox.Show("Данные успешно изменены.");
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancelMain_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
