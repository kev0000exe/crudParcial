using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using DataCustomers; // Importa el namespace donde se encuentra la clase CustomerRepository.
using System.Reflection;

namespace BDcrud
{
    public partial class Form1 : Form
    {
        // Instancia del repositorio de clientes para realizar operaciones CRUD.
        CustomerRepositorys customerRepository = new CustomerRepositorys();

        // Constructor del formulario.
        public Form1()
        {
            InitializeComponent(); // Inicializa los componentes del formulario.
        }

        // Cargar todos los clientes y mostrarlos en el DataGridView.
        private void btnCargar_Click(object sender, EventArgs e)
        {
            var customers = customerRepository.ObtenerTodos(); // Obtiene todos los clientes.
            datagrid.DataSource = customers; // Asigna la lista de clientes al DataGridView.
        }

        // Buscar un cliente por su ID y mostrar sus datos en los TextBoxes.
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var cliente = customerRepository.ObtenerPorID(txtBuscar.Text); // Busca un cliente por su ID.

            // Muestra los datos del cliente en los TextBoxes.
            txtCustomerID.Text = cliente.CustomerID;
            txtCompanyName.Text = cliente.CompanyName;
            txtContactName.Text = cliente.ContactName;
            txtContactTitle.Text = cliente.ContactTitle;
            txtAddress.Text = cliente.Address;
            txtCity.Text = cliente.City;
            txtRegion.Text = cliente.Region;
            txtPostalCode.Text = cliente.PostalCode;
            txtCountry.Text = cliente.Country;
            txtPhone.Text = cliente.Phone;
            txtFax.Text = cliente.Fax;
        }

        // Insertar un nuevo cliente en la base de datos.
        private void btnInsertar_Click(object sender, EventArgs e)
        {
            var resultado = 0; // Almacena el resultado de la operación de inserción.

            var nuevoCliente = ObtenerNuevoCliente(); // Obtiene un objeto con los datos del nuevo cliente.

            // Verifica si hay campos vacíos en el nuevo cliente.
            if (!ValidarCampoNull(nuevoCliente))
            {
                // Si no hay campos vacíos, agrega el cliente a la base de datos.
                resultado = customerRepository.AgregarCliente(nuevoCliente);
                MessageBox.Show("Cliente guardado correctamente. Filas afectadas: " + resultado);
            }
            else
            {
                MessageBox.Show("Debe completar todos los campos.");
            }
        }

        // Valida si alguno de los campos del cliente es nulo o está vacío.
        public bool ValidarCampoNull(object objeto)
        {
            // Recorre todas las propiedades del objeto cliente.
            foreach (PropertyInfo property in objeto.GetType().GetProperties())
            {
                object value = property.GetValue(objeto, null); // Obtiene el valor de la propiedad.
                if (value == null || (value is string && string.IsNullOrEmpty((string)value)))
                {
                    return true; // Devuelve true si encuentra un campo vacío o nulo.
                }
            }
            return false; // Devuelve false si no encuentra campos vacíos o nulos.
        }

        // Modificar los datos de un cliente existente.
        private void btModificar_Click(object sender, EventArgs e)
        {
            var actualizarCliente = ObtenerNuevoCliente(); // Obtiene un objeto con los datos actualizados del cliente.
            int actualizadas = customerRepository.ActualizarCliente(actualizarCliente); // Actualiza el cliente en la base de datos.
            MessageBox.Show("Datos actualizados correctamente. Filas afectadas: " + actualizadas);
        }

        // Obtener un nuevo cliente a partir de los datos ingresados en los TextBoxes.
        private CustomersGet ObtenerNuevoCliente()
        {
            // Crea una nueva instancia de CustomersGet y asigna los valores de los TextBoxes.
            var nuevoCliente = new CustomersGet
            {
                CustomerID = txtCustomerID.Text,
                CompanyName = txtCompanyName.Text,
                ContactName = txtContactName.Text,
                ContactTitle = txtContactTitle.Text,
                Address = txtAddress.Text,
                City = txtCity.Text,
                Region = txtRegion.Text,
                PostalCode = txtPostalCode.Text,
                Country = txtCountry.Text,
                Phone = txtPhone.Text,
                Fax = txtFax.Text
            };

            return nuevoCliente; // Devuelve el nuevo cliente.
        }

        // Eliminar un cliente de la base de datos, junto con sus pedidos asociados.
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                // Primero, elimina los pedidos asociados al cliente.
                int pedidosEliminados = customerRepository.EliminarPedidosPorCliente(txtCustomerID.Text);

                // Luego, elimina el cliente por su ID.
                int clientesEliminados = customerRepository.EliminarCliente(txtCustomerID.Text);

                // Muestra un mensaje de éxito con el número de filas eliminadas.
                MessageBox.Show($"Cliente eliminado correctamente. Pedidos eliminados: {pedidosEliminados}, Clientes eliminados: {clientesEliminados}");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al intentar eliminar el cliente: " + ex.Message);
            }
        }
    }
}
