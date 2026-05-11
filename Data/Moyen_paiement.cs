using MySql.Data.MySqlClient;
using RepositoryPOO;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Bibliotheque_classe_projet_Seux
{
    public class Moyen_paiement : BDDObject
    {
        private string nom_moyen_paiement;

        public string Nom_moyen_paiement { get => nom_moyen_paiement; set => nom_moyen_paiement = value; }

        public Moyen_paiement() { }

        public Moyen_paiement(string nom_moyen_paiement) : base()
        {
            this.Nom_moyen_paiement = nom_moyen_paiement;
        }

        public override string GetTableName() => "Moyen_paiement";

        public override Dictionary<string, string> GetInsertUpdateColumns()
        {
            return new Dictionary<string, string>
            {
                { "nom_moyen_paiement", "@nom_moyen_paiement" }
            };
        }

        public override List<IDataParameter> GetInsertUpdateParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@nom_moyen_paiement", MySqlDbType.VarChar) { Value = this.Nom_moyen_paiement }
            };
        }

        public override Dictionary<string, string> GetPrimaryKeyColumn()
        {
            return new Dictionary<string, string> { { "id_moyen_paiement", "@id_moyen_paiement" } };
        }

        public override List<IDataParameter> GetPrimaryKeyParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@id_moyen_paiement", MySqlDbType.Int32) { Value = Id }
            };
        }

        public override void FillWithDataReader(DbDataReader reader)
        {
            base.FillWithDataReader(reader);
            this.Nom_moyen_paiement = reader["nom_moyen_paiement"].ToString();
        }

        public override Dictionary<string, string> GetPredicateColumns()
        {
            var columns = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Nom_moyen_paiement))
                columns.Add("nom_moyen_paiement", "@nom_moyen_paiement");
            return columns;
        }

        public override List<IDataParameter> GetPredicateParameters()
        {
            var parameters = new List<IDataParameter>();
            if (!string.IsNullOrEmpty(Nom_moyen_paiement))
                parameters.Add(new MySqlParameter("@nom_moyen_paiement", MySqlDbType.VarChar) { Value = this.Nom_moyen_paiement });
            return parameters;
        }

        public override IBDDConnector getNewInstance() => new Moyen_paiement();
    }
}