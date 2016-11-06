﻿using ClinicaFrba.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClinicaFrba.Abm_Afiliado.Modifiacion
{
    public partial class MotivoCambioPlan : Form
    {
        int afiliado;

        public MotivoCambioPlan(int afiliado)
        {
            this.afiliado = afiliado;

            InitializeComponent();
        }

        private void botonAtras_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void botonAceptar_Click(object sender, EventArgs e)
        {
            Utils.registrarMotivoModificacion(this.afiliado, this.richTextBox1.Text);
            MessageBox.Show("El motivo fue registrado");
            this.Close();
        }
    }
}
