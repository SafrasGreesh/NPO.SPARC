using Npgsql;
using NPO.SPARC._2.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
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
using NPO.SPARC._2.Entity;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace NPO.SPARC._2
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Host=localhost;Port=5432;Database=NPO.SPARC;Username=admin;Password=1";

        public MainWindow()
        {
            tabControl = new TabControl();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (var context = new ApplicationContext())
            {
                var tests = context.Tests.ToList();
                testsDataGrid.ItemsSource = tests;

                var parameters = context.Parameters.ToList();
                parametersDataGrid.ItemsSource = parameters;
            }
        }

        private void DeleteParameterButton_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = (Button)sender;
            Parameters parameter = (Parameters)deleteButton.DataContext;
            int parameterId = parameter.ParameterId;

            if (parameterId == 0)
            {
                return;
            }

            using (var context = new ApplicationContext())
            {
                var parameterToDelete = context.Parameters.Find(parameterId);
                if (parameterToDelete != null)
                {
                    context.Parameters.Remove(parameterToDelete);
                }

                context.SaveChanges();

                parametersDataGrid.ItemsSource = context.Parameters.ToList();
            }
            RefreshDataGrids();
        }

        private void DeleteTestButton_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = (Button)sender;
            Tests test = deleteButton.DataContext as Tests;

            if (test == null || test.TestId == 0)
            {
                return;
            }

            using (var context = new ApplicationContext())
            {
                var testToDelete = context.Tests.Include(t => t.Parameters).SingleOrDefault(t => t.TestId == test.TestId);

                if (testToDelete != null)
                {
                    if (testToDelete.Parameters.Any())
                    {
                        MessageBoxResult result = MessageBox.Show("Тест связан с параметрами. Хотите удалить его со всеми параметрами?", "Подтверждение удаления", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            context.Parameters.RemoveRange(testToDelete.Parameters);
                        }
                        else
                        {
                            return;
                        }
                    }

                    context.Tests.Remove(testToDelete);
                    context.SaveChanges();
                    testsDataGrid.ItemsSource = context.Tests.ToList();
                }
            }

            RefreshDataGrids();
        }



        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            Tests selectedTest = (Tests)testsDataGrid.SelectedItem;
            Parameters selectedParameter = (Parameters)parametersDataGrid.SelectedItem;

            if (selectedTest != null)
            {
                int testId = selectedTest.TestId;
                SaveChanges(testsDataGrid, parametersDataGrid, testId);
            }
            else if (selectedParameter != null)
            {
                int testId = selectedParameter.TestId;
                SaveChanges(testsDataGrid, parametersDataGrid, testId);
            }

            RefreshDataGrids();
        }

        private void RefreshDataGrids()
        {
            using (var context = new ApplicationContext())
            {
                var tests = context.Tests.ToList();
                testsDataGrid.ItemsSource = tests;
                testsDataGrid.Items.Refresh();

                var parameters = context.Parameters.ToList();
                parametersDataGrid.ItemsSource = parameters;
                parametersDataGrid.Items.Refresh();
            }
        }


        private void SaveChanges(DataGrid testsDataGrid, DataGrid parametersDataGrid, int testId)
        {
            List<Tests> tests = (List<Tests>)testsDataGrid.ItemsSource;
            List<Parameters> parameters = (List<Parameters>)parametersDataGrid.ItemsSource;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string insertTestQuery =
                        "INSERT INTO \"Tests\" (\"TestDate\", \"BlockName\", \"Note\") " +
                        "VALUES (@testDate, @blockName, @note)";
                    using (NpgsqlCommand insertTestCommand = new NpgsqlCommand(insertTestQuery, connection))
                    {
                        insertTestCommand.Parameters.Clear();
                        foreach (Tests test in tests)
                        {
                            if (test.TestId == 0) 
                            {
                                insertTestCommand.Parameters.AddWithValue("@testDate", test.TestDate);
                                insertTestCommand.Parameters.AddWithValue("@blockName", test.BlockName);
                                insertTestCommand.Parameters.AddWithValue("@note", test.Note);
                                insertTestCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    if (parameters.Count > 0)
                    {
                        string insertParametersQuery =
                            "INSERT INTO \"Parameters\" (\"TestId\", \"ParameterName\", \"RequiredValue\", \"MeasuredValue\") " +
                            "VALUES (@testId, @parameterName, @requiredValue, @measuredValue)";
                        using (NpgsqlCommand insertParametersCommand = new NpgsqlCommand(insertParametersQuery, connection))
                        {
                            insertParametersCommand.Parameters.Clear();
                            foreach (Parameters parameter in parameters)
                            {
                                if (parameter.ParameterId == 0)
                                {
                                    insertParametersCommand.Parameters.AddWithValue("@testId", testId);
                                    insertParametersCommand.Parameters.AddWithValue("@parameterName", parameter.ParameterName);
                                    insertParametersCommand.Parameters.AddWithValue("@requiredValue", parameter.RequiredValue);
                                    insertParametersCommand.Parameters.AddWithValue("@measuredValue", parameter.MeasuredValue);
                                    insertParametersCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при сохранении данных: " + ex.Message, "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных: " + ex.Message, "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }




    }
}