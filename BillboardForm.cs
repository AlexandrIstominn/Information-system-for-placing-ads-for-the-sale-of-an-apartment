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
    public partial class BillboardForm : Form
    {
        private MySqlConnection connection;
        private DataTable originalDataTable;
        public BillboardForm(MySqlConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            LoadData();
            txtSearch.TextChanged += txtSearch_TextChanged;
        }

        private void BillboardForm_Load(object sender, EventArgs e)
        {
            LoadData();
            originalDataTable = (DataTable)dataGridView1.DataSource;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchValue = txtSearch.Text.Trim();
            DataView dataView = originalDataTable.DefaultView;
            StringBuilder filterExpression = new StringBuilder();
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                if (dataGridView1.Columns[i].ValueType == typeof(string))
                {
                    if (filterExpression.Length > 0)
                    {
                        filterExpression.Append(" OR ");
                    }
                    filterExpression.Append($"CONVERT([{dataGridView1.Columns[i].Name}], 'System.String') LIKE '%{searchValue}%'");
                }
                else if (dataGridView1.Columns[i].ValueType == typeof(int) || dataGridView1.Columns[i].ValueType == typeof(double) || dataGridView1.Columns[i].ValueType == typeof(decimal))
                {
                    if (filterExpression.Length > 0)
                    {
                        filterExpression.Append(" OR ");
                    }
                    filterExpression.Append($"CONVERT([{dataGridView1.Columns[i].Name}], 'System.String') LIKE '%{searchValue}%'");
                }
            }
            dataView.RowFilter = filterExpression.ToString();
            DataTable filteredDataTable = dataView.ToTable();

            if (filteredDataTable.Rows.Count > 0)
            {
                dataGridView1.DataSource = filteredDataTable;
            }
            else
            {
                dataGridView1.DataSource = originalDataTable;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void LoadData()
        {
            try
            {
                string query = "SELECT * from доска_объявлений";
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

        private void btnBillboard_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex]; 

                string billboardId = selectedRow.Cells["Код_Объявления"].Value.ToString();

                string connectionString = "Server=127.0.0.1;Database=data;Uid=root;Pwd=pbvf28101973;charset=utf8;";
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();
                var frm = new InfoForm(connection, int.Parse(billboardId));
                frm.ShowDialog();
                LoadData();
            }
            else
            {
                MessageBox.Show("Не выбрана строка для редактирования объявления!");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            var frm = new AboutForm();
            frm.ShowDialog();
        }
    }
}

