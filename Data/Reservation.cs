using MySql.Data.MySqlClient;
using RepositoryPOO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Bibliotheque_classe_projet_Seux
{
    public class Reservation : BDDObject
    {
        private DateTime date_reservation;
        private DateTime date_prestation;
        private string rue;
        private int code_postal;
        private string ville;
        private int id_prestation;
        private int id_client;

        public DateTime Date_reservation { get => date_reservation; set => date_reservation = value; }
        public DateTime Date_prestation { get => date_prestation; set => date_prestation = value; }
        public string Rue { get => rue; set => rue = value; }
        public int Code_postal { get => code_postal; set => code_postal = value; }
        public string Ville { get => ville; set => ville = value; }
        public int Id_prestation { get => id_prestation; set => id_prestation = value; }
        public int Id_client { get => id_client; set => id_client = value; }

        public Reservation() { }

        public Reservation(DateTime date_reservation, DateTime date_prestation, string rue, int code_postal, string ville, int id_prestation, int id_client) : base()
        {
            this.Date_reservation = date_reservation;
            this.Date_prestation = date_prestation;
            this.Rue = rue;
            this.Code_postal = code_postal;
            this.Ville = ville;
            this.Id_prestation = id_prestation;
            this.Id_client = id_client;
        }

        public override string GetTableName() => "Reservation";

        public override Dictionary<string, string> GetInsertUpdateColumns()
        {
            return new Dictionary<string, string>
            {
                { "date_reservation", "@date_reservation" },
                { "date_prestation", "@date_prestation" },
                { "rue", "@rue" },
                { "code_postal", "@code_postal" },
                { "ville", "@ville" },
                { "id_prestation", "@id_prestation" },
                { "id_client", "@id_client" }
            };
        }

        public override List<IDataParameter> GetInsertUpdateParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@date_reservation", MySqlDbType.Date) { Value = this.Date_reservation },
                new MySqlParameter("@date_prestation", MySqlDbType.Date) { Value = this.Date_prestation },
                new MySqlParameter("@rue", MySqlDbType.VarChar) { Value = this.Rue },
                new MySqlParameter("@code_postal", MySqlDbType.Int32) { Value = this.Code_postal },
                new MySqlParameter("@ville", MySqlDbType.VarChar) { Value = this.Ville },
                new MySqlParameter("@id_prestation", MySqlDbType.Int32) { Value = this.Id_prestation },
                new MySqlParameter("@id_client", MySqlDbType.Int32) { Value = this.Id_client }
            };
        }

        public override Dictionary<string, string> GetPrimaryKeyColumn()
        {
            return new Dictionary<string, string> { { "id_reservation", "@id_reservation" } };
        }

        public override List<IDataParameter> GetPrimaryKeyParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@id_reservation", MySqlDbType.Int32) { Value = Id }
            };
        }

        public override void FillWithDataReader(DbDataReader reader)
        {
            base.FillWithDataReader(reader);
            this.Date_reservation = Convert.ToDateTime(reader["date_reservation"]);
            this.Date_prestation = Convert.ToDateTime(reader["date_prestation"]);
            this.Rue = reader["rue"].ToString();
            this.Code_postal = Convert.ToInt32(reader["code_postal"]);
            this.Ville = reader["ville"].ToString();
            this.Id_prestation = Convert.ToInt32(reader["id_prestation"]);
            this.Id_client = Convert.ToInt32(reader["id_client"]);
        }

        public override Dictionary<string, string> GetPredicateColumns()
        {
            var columns = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Ville)) columns.Add("ville", "@ville");
            if (Id_prestation > 0) columns.Add("id_prestation", "@id_prestation");
            if (Id_client > 0) columns.Add("id_client", "@id_client");
            return columns;
        }

        public override List<IDataParameter> GetPredicateParameters()
        {
            var parameters = new List<IDataParameter>();
            if (!string.IsNullOrEmpty(Ville))
                parameters.Add(new MySqlParameter("@ville", MySqlDbType.VarChar) { Value = this.Ville });
            if (Id_prestation > 0)
                parameters.Add(new MySqlParameter("@id_prestation", MySqlDbType.Int32) { Value = this.Id_prestation });
            if (Id_client > 0)
                parameters.Add(new MySqlParameter("@id_client", MySqlDbType.Int32) { Value = this.Id_client });
            return parameters;
        }

        public override IBDDConnector getNewInstance() => new Reservation();
    }
}