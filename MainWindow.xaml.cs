using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoFinal4
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<MovimientoCaja> movimientosCaja = new ObservableCollection<MovimientoCaja>();
        private ObservableCollection<Transaccion> transacciones = new ObservableCollection<Transaccion>();
        private ObservableCollection<Producto> inventario = new ObservableCollection<Producto>();

        public MainWindow()
        {
            InitializeComponent();

            dgTransacciones.ItemsSource = transacciones;
            dgInventario.ItemsSource = inventario;
            dgMovimientos.ItemsSource = movimientosCaja;

            cmbTipoTransaccion.SelectionChanged += cmbTipoTransaccion_SelectionChanged;
        }

        private void cmbTipoTransaccion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = cmbTipoTransaccion.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string tipoTransaccion = selectedItem.Content.ToString();
                lblCantidad.Visibility = (tipoTransaccion == "Compra" || tipoTransaccion == "Venta") ? Visibility.Visible : Visibility.Collapsed;
                txtCantidad.Visibility = (tipoTransaccion == "Compra" || tipoTransaccion == "Venta") ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void RegistrarTransaccion_Click(object sender, RoutedEventArgs e)
        {
            string tipo = (cmbTipoTransaccion.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(tipo))
            {
                MessageBox.Show("Por favor, seleccione un tipo de transacción.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string descripcion = txtDescripcion.Text;
            decimal monto;
            decimal cantidad;

            if (!decimal.TryParse(txtMonto.Text, out monto))
            {
                MessageBox.Show("Por favor, ingrese un monto válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(txtCantidad.Text, out cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Por favor, ingrese una cantidad válida para la transacción.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string tipoMoneda = txtTipoMoneda.Text;

            // Añadir la transacción
            var transaccion = new Transaccion { Tipo = tipo, Descripcion = descripcion, Monto = monto, Cantidad = cantidad, TipoMoneda = tipoMoneda };
            transacciones.Add(transaccion);

            // Actualizar el inventario si es una compra o venta
            if (tipo == "Compra" || tipo == "Venta")
            {
                var productoExistente = inventario.FirstOrDefault(p => p.Nombre == descripcion);
                if (productoExistente != null)
                {
                    productoExistente.Cantidad += (tipo == "Compra") ? cantidad : -cantidad;
                }
                else if (tipo == "Compra")
                {
                    inventario.Add(new Producto { Nombre = descripcion, Cantidad = cantidad, PrecioCompra = monto });
                }
            }

            // Limpiar los campos
            txtDescripcion.Text = "";
            txtMonto.Text = "";
            txtCantidad.Text = "";
        }

        private void AgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombreProducto.Text;
            decimal cantidad, precioCompra, precioVenta;

            if (!decimal.TryParse(txtCantidadProducto.Text, out cantidad))
            {
                MessageBox.Show("Por favor, ingrese una cantidad válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(txtPrecioCompra.Text, out precioCompra))
            {
                MessageBox.Show("Por favor, ingrese un precio de compra válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(txtPrecioVenta.Text, out precioVenta))
            {
                MessageBox.Show("Por favor, ingrese un precio de venta válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var producto = new Producto
            {
                Nombre = nombre,
                Cantidad = cantidad,
                PrecioCompra = precioCompra,
                PrecioVenta = precioVenta,
                FechaCaducidad = dpFechaCaducidad.SelectedDate,
                NumeroLote = txtNumeroLote.Text
            };

            inventario.Add(producto);

            // Limpiar los campos
            txtNombreProducto.Text = "";
            txtCantidadProducto.Text = "";
            txtPrecioCompra.Text = "";
            txtPrecioVenta.Text = "";
            dpFechaCaducidad.SelectedDate = null;
            txtNumeroLote.Text = "";
        }

        private void RegistrarMovimiento_Click(object sender, RoutedEventArgs e)
        {
            string tipoMovimiento = (cmbTipoMovimiento.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(tipoMovimiento))
            {
                MessageBox.Show("Por favor, seleccione un tipo de movimiento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string descripcion = txtDescripcionMovimiento.Text;
            decimal monto;

            if (!decimal.TryParse(txtMontoMovimiento.Text, out monto))
            {
                MessageBox.Show("Por favor, ingrese un monto válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var movimiento = new MovimientoCaja
            {
                TipoMovimiento = tipoMovimiento,
                Descripcion = descripcion,
                Monto = monto,
                Fecha = DateTime.Now
            };

            movimientosCaja.Add(movimiento);

            // Limpiar los campos
            txtDescripcionMovimiento.Text = "";
            txtMontoMovimiento.Text = "";
        }
    }
}
