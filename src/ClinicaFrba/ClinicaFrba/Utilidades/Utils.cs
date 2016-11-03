﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using ClinicaFrba.Abm_Afiliado;

namespace ClinicaFrba.Utilidades
{
    public static class Utils
    {
        static public void llenar(ComboBox combo, List<KeyValuePair<int,string>> items)
        {
            combo.DisplayMember = "Value";
            combo.ValueMember = "Key";

            items.ForEach(item => combo.Items.Add(item));

            if (combo.Items.Count > 0)
                combo.SelectedItem = combo.Items[0];
        }

        static public void llenar(ListBox list, List<KeyValuePair<int,string>> items)
        {
            list.DisplayMember = "Value";
            list.ValueMember = "Key";

            items.ForEach(item => list.Items.Add(item));

            if (list.Items.Count > 0)
                list.SelectedItem = list.Items[0];
        }

        static public SqlCommand crearSp(string nombreSp, List<KeyValuePair<string, object>> parametros, SqlConnection conexion)
        { //Con parametros
            SqlCommand query = new SqlCommand(nombreSp, conexion);
            query.CommandType = CommandType.StoredProcedure;
            parametros.ForEach(pair => query.Parameters.Add(new SqlParameter(pair.Key, pair.Value)));
            return query;
        }

        static public SqlCommand crearSp(string nombreSp, SqlConnection conexion) 
        { //Sin parametros explicitos
            SqlCommand query = new SqlCommand(nombreSp, conexion);
            query.CommandType = CommandType.StoredProcedure;
            return query;
        }



        static public int getNumeroAfiliadoDesdeUsuario(string usuario)
        {
            var conexion = DBConnection.getConnection();

            int userId;
            Int32.TryParse(usuario, out userId);

            SqlCommand comando = new SqlCommand("select afil_id From CLINICA.Afiliados Where afil_usuario = @user", conexion);

            comando.Parameters.AddWithValue("@user", userId);

            conexion.Open();

            SqlDataReader reader = comando.ExecuteReader();

            if (reader.Read())
            {
                return Convert.ToInt32(reader[0]); //afil_plan
            }
            return -1;
        }


        static public int superaLimiteSemanal(int profesional) {
            var conexion = DBConnection.getConnection();

            SqlCommand comando = new SqlCommand("select top 1 count(*)/2 from Clinica.horarios where hora_profesional = @profesional group by hora_profesional, datepart(WEEK,hora_fecha) order by count(*)/2 DESC", conexion);
            // divido por dos porque tiene 2 horarios por hora, porque cada uno dura media hora.
            // la cosa de la fecha del final agrupa las cosas si tienen igual profesional e igual semana.

            comando.Parameters.AddWithValue("@profesional", profesional);

            conexion.Open();

            SqlDataReader reader = comando.ExecuteReader();

            if (reader.Read()) {
                return Convert.ToInt32(reader[0]);
            }
            return -1;
        }


        static public int buscarPlanDeAfiliado(int afiliado)
        {
            var conexion = DBConnection.getConnection();

            //int afilId;
            //Int32.TryParse(afiliado, out afilId);

            SqlCommand comando = new SqlCommand("select afil_plan From CLINICA.Afiliados Where afil_id = @afiliado", conexion);

            comando.Parameters.AddWithValue("@afiliado", afiliado); //OJO, TIENEN QUE LLAMARSE IGUAL

            conexion.Open();

            SqlDataReader reader = comando.ExecuteReader();

            if (reader.Read())
            {
                return Convert.ToInt32(reader[0]); //afil_plan
            }
            return -1;
        }

        static public int buscarPrecioPlan(int plan)
        {
            var conexion = DBConnection.getConnection();

            SqlCommand comando = new SqlCommand("select plan_precioBono From CLINICA.Planes Where plan_id = @plan", conexion);
            comando.Parameters.AddWithValue("@plan", plan);

            conexion.Open();
            SqlDataReader reader = comando.ExecuteReader();

            if (reader.Read())
            {
                return Convert.ToInt32(reader[0]);//plan_precioBono
            }
            return -1;
        }

        static public DataTable getProfesionales() //Hecho para pedir turno, se puede tomar la forma y cambiar!
        {
            var conexion = DBConnection.getConnection();
            SqlCommand comando = new SqlCommand("select p.prof_id as Profesional, u.usua_apellido as Apellido, u.usua_nombre as Nombre, e.espe_nombre as Especialidad from CLINICA.Profesionales p, CLINICA.Usuarios u, CLINICA.EspecialidadXProfesional espe, CLINICA.Especialidades e WHERE p.prof_usuario=u.usua_id AND espe.prof_id = p.prof_id AND espe.espe_id = e.espe_id", conexion);
            comando.CommandType = CommandType.Text;

            SqlDataAdapter sqlDataAdap = new SqlDataAdapter(comando);
            DataTable tabla = new DataTable();
            sqlDataAdap.Fill(tabla);

            return tabla;
        }

        static public DataTable getEspecialidades()
{
            var conexion = DBConnection.getConnection();
            SqlCommand comando = new SqlCommand("select e.espe_id as Especialidad, e.espe_nombre from CLINICA.Especialidades e", conexion);
            comando.CommandType = CommandType.Text;

            SqlDataAdapter sqlDataAdap = new SqlDataAdapter(comando);
            DataTable tabla = new DataTable();
            sqlDataAdap.Fill(tabla);

            return tabla;
        }

        static public DataTable getProfesionalesDeEspecialidad(string filtroEspe) 
        {
            var conexion = DBConnection.getConnection();
            SqlCommand comando = new SqlCommand("select p.prof_id as Profesional, u.usua_apellido as Apellido, u.usua_nombre as Nombre, e.espe_nombre as Especialidad from CLINICA.Profesionales p, CLINICA.Usuarios u, CLINICA.EspecialidadXProfesional espe, CLINICA.Especialidades e WHERE p.prof_usuario=u.usua_id AND espe.prof_id = p.prof_id AND espe.espe_id = e.espe_id AND e.espe_nombre=@filtroEspe", conexion);
            comando.Parameters.AddWithValue("@filtroEspe", filtroEspe);
            comando.CommandType = CommandType.Text;

            SqlDataAdapter sqlDataAdap = new SqlDataAdapter(comando);
            DataTable tabla = new DataTable();
            sqlDataAdap.Fill(tabla);

            return tabla;
        }

        static public DataTable getHorariosDelProfesional(string profesional)
        {
            var conexion = DBConnection.getConnection();
            SqlCommand comando = new SqlCommand("select hora_id as IdHorario, hora_fecha Dia, hora_inicio Hora from CLINICA.Horarios where hora_profesional = @profesional", conexion);
            comando.Parameters.AddWithValue("@profesional", Int32.Parse(profesional));
            comando.CommandType = CommandType.Text;

            SqlDataAdapter sqlDataAdap = new SqlDataAdapter(comando);
            DataTable tabla = new DataTable();
            sqlDataAdap.Fill(tabla);

            return tabla;
        }

        static public int getIdDesdePlan(string plan)
        {
            var conexion = DBConnection.getConnection();
            SqlCommand comando = new SqlCommand("SELECT plan_id from CLINICA.Planes WHERE plan_nombre = @plan", conexion);
            comando.Parameters.AddWithValue("@plan", plan);
            comando.CommandType = CommandType.Text;

            conexion.Open();
            SqlDataReader reader = comando.ExecuteReader();
            if (reader.Read())
            {
                return Convert.ToInt32(reader[0]);
            }
            return -1;
        }


        static public long getIdDesdeUserName(string username)
        {
            var conexion = DBConnection.getConnection();
            SqlCommand comando = new SqlCommand("SELECT usua_id FROM CLINICA.Usuarios WHERE usua_username = @username", conexion);
            comando.Parameters.AddWithValue("@username", username);
            comando.CommandType = CommandType.Text;

            conexion.Open();
            SqlDataReader reader = comando.ExecuteReader();
            if (reader.Read())
            {
                return Convert.ToInt64(reader[0]);
            }
            return -1;
        }



        //INGRESO VALORES A LA BD

        static public void registarUsuario(Afiliado afiliado){

            var conexion = DBConnection.getConnection();

            SqlCommand comando = new SqlCommand("CLINICA.ingresarUsuario", conexion);

            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@username", afiliado.getUsername());
            comando.Parameters.AddWithValue("@password", afiliado.getPassword());
            comando.Parameters.AddWithValue("@nombre", afiliado.getNombre());
            comando.Parameters.AddWithValue("@apellido", afiliado.getApellido());
            comando.Parameters.AddWithValue("@tipoDoc", afiliado.getTipoDoc());
            comando.Parameters.AddWithValue("@nroDoc", afiliado.getNroDoc());
            comando.Parameters.AddWithValue("@direccion", afiliado.getDireccion());
            comando.Parameters.AddWithValue("@telefono", afiliado.getTelefono());
            comando.Parameters.AddWithValue("@fechaNacimiento", afiliado.getFechaNac());
            comando.Parameters.AddWithValue("@sexo", afiliado.getSexo());
            comando.Parameters.AddWithValue("@mail", afiliado.getMail());

            conexion.Open();
            comando.ExecuteReader();
         
        }

        static public void registrarAfiliado(Afiliado afiliado)
        {
            int codPlan = Utilidades.Utils.getIdDesdePlan(afiliado.getPlan());

            long usuaId = Utilidades.Utils.getIdDesdeUserName(afiliado.getUsername());

            var conexion = DBConnection.getConnection();

            SqlCommand comando = new SqlCommand("CLINICA.ingresarAfiliado", conexion);
            comando.CommandType = CommandType.StoredProcedure;

            //TODO: Ver si lo puedo retornar desde el SP, cuando registro al usuario
            comando.Parameters.AddWithValue("@usuario", usuaId);
            comando.Parameters.AddWithValue("@plan", codPlan);
            comando.Parameters.AddWithValue("@estado", afiliado.getEstadoCivil());
            comando.Parameters.AddWithValue("@hijos", afiliado.getHijosACargo());

            conexion.Open();
            comando.ExecuteReader();
        }


        static public void registrarFamiliarAfiliado(Afiliado afiliado)
        {
            int codPlan = Utilidades.Utils.getIdDesdePlan(afiliado.getPlan());
            long usuaId = Utilidades.Utils.getIdDesdeUserName(afiliado.getUsername());

            var conexion = DBConnection.getConnection();

            SqlCommand comando = new SqlCommand("CLINICA.agregarFamiliar", conexion);
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@afiliado_raiz", afiliado.getCodigoAfiliado());
            comando.Parameters.AddWithValue("@usuario", usuaId);
            comando.Parameters.AddWithValue("@plan", codPlan);
            comando.Parameters.AddWithValue("@estado", afiliado.getEstadoCivil());
            comando.Parameters.AddWithValue("@hijos", afiliado.getHijosACargo());

            conexion.Open();
            comando.ExecuteReader();
        }


        static public void actualizarAfiliado(Afiliado afiliado)
        {
            int codPlan = Utilidades.Utils.getIdDesdePlan(afiliado.getPlan());
            //long usuaId = Utilidades.Utils.getIdDesdeUserName(afiliado.getUsername());

            var conexion = DBConnection.getConnection();

            SqlCommand comando = new SqlCommand("CLINICA.modificarAfiliado", conexion);
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@username", afiliado.getUsuaId());
            comando.Parameters.AddWithValue("@direccion", afiliado.getDireccion());
            comando.Parameters.AddWithValue("@telefono", afiliado.getTelefono());
            comando.Parameters.AddWithValue("@plan", codPlan);
            comando.Parameters.AddWithValue("@estado", afiliado.getEstadoCivil());
            comando.Parameters.AddWithValue("@hijos", afiliado.getHijosACargo());

            conexion.Open();
            comando.ExecuteReader();
        }





        static public int obtenerNumeroAfiliadoRecienRegistrado()
        {
            int afil_id = 0;
            var conexion = DBConnection.getConnection();

            SqlCommand comando = new SqlCommand("SELECT MAX(afil_id) FROM CLINICA.Afiliados ", conexion);
            conexion.Open();
            SqlDataReader reader = comando.ExecuteReader();
            if (reader.Read())
            {
                afil_id = Convert.ToInt32(reader[0]);
            }

            return afil_id;
        }

        static public List<KeyValuePair<int, string>> getPlanes()
        {
            var conexion = DBConnection.getConnection();
            List<KeyValuePair<int, string>> planes = new List<KeyValuePair<int, string>>();

            SqlCommand comando = new SqlCommand("CLINICA.getPlanes", conexion);
            comando.CommandType = CommandType.StoredProcedure;

            conexion.Open();

            SqlDataReader reader = comando.ExecuteReader();
            while (reader.Read())
            {
                planes.Add(new KeyValuePair<int, string>(Int32.Parse(reader["plan_id"].ToString()),
                                                                    reader["plan_nombre"].ToString()));
            }

            return planes;
        }

        static public List<KeyValuePair<int, string>> getRoles(string usuario) {
            var conexion = DBConnection.getConnection();

            List<KeyValuePair<int, string>> rolesAsignados = new List<KeyValuePair<int, string>>();
            SqlCommand comando = new SqlCommand("CLINICA.getRolesUsuario", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@user", usuario);
            conexion.Open();

            SqlDataReader reader = comando.ExecuteReader();
            while (reader.Read()) {
                rolesAsignados.Add(new KeyValuePair<int, string>(Int32.Parse(reader["role_id"].ToString()),
                                                                    reader["role_nombre"].ToString()));
            }

            return rolesAsignados;
        }


        static public SqlDataReader obtenerAfiliadoDesdeUsername(long cod_usuario)
        {

            var conexion = DBConnection.getConnection();

            SqlCommand comando = new SqlCommand("select * From CLINICA.Afiliados Where afil_usuario = @user", conexion);
            comando.Parameters.AddWithValue("@user", cod_usuario);

            conexion.Open();

            SqlDataReader reader = comando.ExecuteReader();

            return reader;
        }

        static public SqlDataReader obtenerUsuarioDesdeUsername(long cod_usuario)
        {

            var conexion = DBConnection.getConnection();

            SqlCommand comando = new SqlCommand("select usua_direccion,usua_tipoDoc, usua_telefono, usua_fechaNacimiento, usua_mail, usua_sexo from CLINICA.Usuarios WHERE usua_id = @user", conexion);
            comando.Parameters.AddWithValue("@user", cod_usuario);

            conexion.Open();

            SqlDataReader reader = comando.ExecuteReader();

            return reader;
        }
    }
}
