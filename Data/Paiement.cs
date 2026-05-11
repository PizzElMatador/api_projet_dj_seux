using MySql.Data.MySqlClient;
using RepositoryPOO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Bibliotheque_classe_projet_Seux
{
    public class Paiement : BDDObject
    {
        private decimal montant;
        private DateTime date_paiement;
        private string statut;
        private int id_moyen_paiement;
        private int id_reservation;

        public decimal Montant { get => montant; set => montant = value; }
        public DateTime Date_paiement { get => date_paiement; set => date_paiement = value; }
        public string Statut { get => statut; set => statut = value; }
        public int Id_moyen_paiement { get => id_moyen_paiement; set => id_moyen_paiement = value; }
        public int Id_reservation { get => id_reservation; set => id_reservation = value; }

        public Paiement() { }

        public Paiement(decimal montant, DateTime date_paiement, string statut, int id_moyen_paiement, int id_reservation) : base()
        {
            this.Montant = montant;
            this.Date_paiement = date_paiement;
            this.Statut = statut;
            this.Id_moyen_paiement = id_moyen_paiement;
            this.Id_reservation = id_reservation;
        }

        public override string GetTableName() => "Paiement";

        public override Dictionary<string, string> GetInsertUpdateColumns()
        {
            return new Dictionary<string, string>
            {
                { "montant", "@montant" },
                { "date_paiement", "@date_paiement" },
                { "statut", "@statut" },
                { "id_moyen_paiement", "@id_moyen_paiement" },
                { "id_reservation", "@id_reservation" }
            };
        }

        public override List<IDataParameter> GetInsertUpdateParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@montant", MySqlDbType.Decimal) { Value = this.Montant },
                new MySqlParameter("@date_paiement", MySqlDbType.Date) { Value = this.Date_paiement },
                new MySqlParameter("@statut", MySqlDbType.VarChar) { Value = this.Statut },
                new MySqlParameter("@id_moyen_paiement", MySqlDbType.Int32) { Value = this.Id_moyen_paiement },
                new MySqlParameter("@id_reservation", MySqlDbType.Int32) { Value = this.Id_reservation }
            };
        }

        public override Dictionary<string, string> GetPrimaryKeyColumn()
        {
            return new Dictionary<string, string> { { "id_paiement", "@id_paiement" } };
        }

        public override List<IDataParameter> GetPrimaryKeyParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@id_paiement", MySqlDbType.Int32) { Value = Id }
            };
        }

        public override void FillWithDataReader(DbDataReader reader)
        {
            base.FillWithDataReader(reader);
            this.Montant = Convert.ToDecimal(reader["montant"]);
            this.Date_paiement = Convert.ToDateTime(reader["date_paiement"]);
            this.Statut = reader["statut"].ToString();
            this.Id_moyen_paiement = Convert.ToInt32(reader["id_moyen_paiement"]);
            this.Id_reservation = Convert.ToInt32(reader["id_reservation"]);
        }

        public override Dictionary<string, string> GetPredicateColumns()
        {
            var columns = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Statut)) columns.Add("statut", "@statut");
            if (Id_moyen_paiement > 0) columns.Add("id_moyen_paiement", "@id_moyen_paiement");
            if (Id_reservation > 0) columns.Add("id_reservation", "@id_reservation");
            return columns;
        }

        public override List<IDataParameter> GetPredicateParameters()
        {
            var parameters = new List<IDataParameter>();
            if (!string.IsNullOrEmpty(Statut))
                parameters.Add(new MySqlParameter("@statut", MySqlDbType.VarChar) { Value = this.Statut });
            if (Id_moyen_paiement > 0)
                parameters.Add(new MySqlParameter("@id_moyen_paiement", MySqlDbType.Int32) { Value = this.Id_moyen_paiement });
            if (Id_reservation > 0)
                parameters.Add(new MySqlParameter("@id_reservation", MySqlDbType.Int32) { Value = this.Id_reservation });
            return parameters;
        }

        public override IBDDConnector getNewInstance() => new Paiement();
    }
}