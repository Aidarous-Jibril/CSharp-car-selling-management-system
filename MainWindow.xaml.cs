using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;


namespace WPF_CarSellingManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //initialize sqlconn here after using Data.sqlClient namespace
        SqlConnection sqlConnection; 

        public MainWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["WPF_CarSellingManager.Properties.Settings.PanjuDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);

            //call the methods
            showCompanies();
            showAllCars();
        }


        //CRUD FOR COMPANY LIST
        //create a new method that shows all COMPANIES 
        private void showCompanies()
        {
            try
            {
                String query = "Select * from CarSellingCompany";
                //sqldataAdapter takes care of opening & closing db
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable companyTable = new DataTable();

                    sqlDataAdapter.Fill(companyTable);

                    //Info/values to show in listBox
                    listCompanies.DisplayMemberPath = "Location";
                    listCompanies.SelectedValuePath = "Id";
                    listCompanies.ItemsSource = companyTable.DefaultView;

                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //Add btn event click, Add COMPANY to COMPANY List
        private void AddCompany_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into carSellingCompany values (@Location)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            finally
            {
                sqlConnection.Close();
                showCompanies();
            }


        }

        //Delete btn event click, Delete COMPANY from COMPANY lsit
        private void DeleteCompany_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from carSellingCompany where id = @carSellingCompanyId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@carSellingCompanyId", listCompanies.SelectedValue);
                sqlCommand.ExecuteScalar();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            finally
            {
                sqlConnection.Close();
                showCompanies();
            }
        }

        //UPDATE btn event click, UPDATE COMPANY from COMPANY lsit
        private void UpdateCompany_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update carSellingCompany set Location = @Location where Id = @carSellingCompanyId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@carSellingCompanyId", listCompanies.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            finally
            {
                sqlConnection.Close();
                showCompanies();
            }
        }



        //CRUD FOR RELATION TABLE
        //Here create a new method that SHOW all CAR 
        private void AssociatedCars()
        {
            try
            {

                String query = "Select * from Car c inner join CompanyCar cc on c.Id = cc.CarId WHERE cc.CarSellingCompanyId = @CarSellingCompanyId";

                //this time sqlCommand takes care query & sqlconnection instead of sqlAdapter
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                //sqldataAdapter takes care of opening & closing db
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@CarSellingCompanyId", listCompanies.SelectedValue);
                    DataTable AssociatedCarTable = new DataTable();

                    sqlDataAdapter.Fill(AssociatedCarTable);

                    //Info/values to show in listBox
                    listAssociatedCars.DisplayMemberPath = "Model";
                    listAssociatedCars.SelectedValuePath = "Id";
                    listAssociatedCars.ItemsSource = AssociatedCarTable.DefaultView;

                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //This method calls AssociatedCars(); 
        private void listCompanies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Call methods
            AssociatedCars();
            //calling text box updating text method, CarSellingCompany.
            ShowSelectedcarSellingCompanyInTextBox();
        }


        //Add btn event click, Add CAR & CARSELLINGCOMPANY to COMPANYCAR TABLE
        private void AddCarToCompany_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into CompanyCar values (@CarId, @CarSellingCompanyId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@CarId", listAllCars.SelectedValue );
                sqlCommand.Parameters.AddWithValue("@CarSellingCompanyId", listCompanies.SelectedValue);
                sqlCommand.ExecuteScalar();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            finally
            {
                sqlConnection.Close();
                AssociatedCars();
            }

        }


        //Delete btn event click, Remove/Delete a CAR from the whole system
        private void DeleteCarFromSystem_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                String query = "delete  from Car c inner join CompanyCar cc on c.Id = cc.CarId WHERE cc.CarSellingCompanyId = @CarSellingCompanyId";

                //this time sqlCommand takes care query & sqlconnection instead of sqlAdapter
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                //sqldataAdapter takes care of opening & closing db
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@CarSellingCompanyId", listCompanies.SelectedValue);
                    DataTable AssociatedCarTable = new DataTable();

                    sqlDataAdapter.Fill(AssociatedCarTable);

                    //Info/values to show in listBox
                    listAssociatedCars.DisplayMemberPath = "Model";
                    listAssociatedCars.SelectedValuePath = "Id";
                    listAssociatedCars.ItemsSource = AssociatedCarTable.DefaultView;

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }



        //CRUD FOR CAR
        //create a new method that SHOW all CARS 
        private void showAllCars()
        {
            try
            {
                String query = "Select * from car";
                //sqldataAdapter takes care of opening & closing db
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable carTable = new DataTable();

                    sqlDataAdapter.Fill(carTable);

                    //Info/values to show in listBox
                    listAllCars.DisplayMemberPath = "Model";
                    listAllCars.SelectedValuePath = "Id";
                    listAllCars.ItemsSource = carTable.DefaultView;

                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }


        //Add btn event click, Add CAR to Compnay
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into car values (@Model)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Model", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            finally
            {
                sqlConnection.Close();
                showAllCars();
            }
        }


        //Delete btn event click, Delete CAR from Carlist
        private void DeleteCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from car where id = @carId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@CarId", listAllCars.SelectedValue);
                sqlCommand.ExecuteScalar();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            finally
            {
                sqlConnection.Close();
                showAllCars();
            }
        }

        //UPDATE btn event click, UPDATE CAR from CAR lsit
        private void UpdateCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update car set Model = @Model where Id = @CarId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@CarId", listAllCars.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Model", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            finally
            {
                sqlConnection.Close();
                showAllCars();
            }
        }




        //FOR TEXT BOX UPDATING SELECTED TEXT
        //Create ShowSelectedcarSellingCompanyInTextBox method for TEXT BOX
        private void ShowSelectedcarSellingCompanyInTextBox()
        {
            try
            {
                String query = "Select location from CarSellingCompany where Id = @CarSellingCompanyId";
                //sqldataAdapter takes care of opening & closing db
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@CarSellingCompanyId", listCompanies.SelectedValue);
                    DataTable carSellingTextBoxCompanyTable = new DataTable();
                    sqlDataAdapter.Fill(carSellingTextBoxCompanyTable);

                    myTextBox.Text = carSellingTextBoxCompanyTable.Rows[0]["Location"].ToString();
                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //Create ShowSelectedcarInTextBox method for TEXT BOX
        private void ShowSelectedCarInTextBox()
        {
            try
            {
                String query = "Select model from car where Id = @CarId";
                //sqldataAdapter takes care of opening & closing db
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@CarId", listAllCars.SelectedValue);
                    DataTable carSellingTextBoxCompanyTable = new DataTable();
                    sqlDataAdapter.Fill(carSellingTextBoxCompanyTable);

                    myTextBox.Text = carSellingTextBoxCompanyTable.Rows[0]["Model"].ToString();
                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //Calling ShowSelectedCarInTextBox method for updating TEXT BOX TEXT
        private void listAllCars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedCarInTextBox();
        }


    }
}
