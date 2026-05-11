using MySql.Data.MySqlClient;
using RepositoryPOO;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Bibliotheque_classe_projet_Seux
{
    public class Prestation : BDDObject
    {
        private string? nom_prestation;
        private string? description_presta;
        private decimal prix;
        private int id_type;

        public string? Nom_prestation { get => nom_prestation; set => nom_prestation = value; }
        public string? Description_presta { get => description_presta; set => description_presta = value; }
        public decimal Prix { get => prix; set => prix = value; }
        public int Id_type { get => id_type; set => id_type = value; }

        public Prestation() { }

        public Prestation(string nom_prestation, string description_presta, decimal prix, int id_type) : base()
        {
            this.Nom_prestation = nom_prestation;
            this.Description_presta = description_presta;
            this.Prix = prix;
            this.Id_type = id_type;
        }

        public override string GetTableName() => "Prestation";

        public override Dictionary<string, string> GetInsertUpdateColumns()
        {
            return new Dictionary<string, string>
            {
                { "nom_prestation", "@nom_prestation" },
                { "description_presta", "@description_presta" },
                { "prix", "@prix" },
                { "id_type", "@id_type" }
            };
        }

        public override List<IDataParameter> GetInsertUpdateParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@nom_prestation", MySqlDbType.VarChar) { Value = this.Nom_prestation },
                new MySqlParameter("@description_presta", MySqlDbType.VarChar) { Value = this.Description_presta },
                new MySqlParameter("@prix", MySqlDbType.Decimal) { Value = this.Prix },
                new MySqlParameter("@id_type", MySqlDbType.Int32) { Value = this.Id_type }
            };
        }

        public override Dictionary<string, string> GetPrimaryKeyColumn()
        {
            return new Dictionary<string, string> { { "id_prestation", "@id_prestation" } };
        }

        public override List<IDataParameter> GetPrimaryKeyParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@id_prestation", MySqlDbType.Int32) { Value = Id }
            };
        }

        public override void FillWithDataReader(DbDataReader reader)
        {
            base.FillWithDataReader(reader);
            this.Nom_prestation = reader["nom_prestation"].ToString();
            this.Description_presta = reader["description_presta"].ToString();
            this.Prix = System.Convert.ToDecimal(reader["prix"]);
            this.Id_type = System.Convert.ToInt32(reader["id_type"]);
        }

        public override Dictionary<string, string> GetPredicateColumns()
        {
            var columns = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Nom_prestation))
                columns.Add("nom_prestation", "@nom_prestation");
            if (Id_type > 0)
                columns.Add("id_type", "@id_type");
            return columns;
        }

        public override List<IDataParameter> GetPredicateParameters()
        {
            var parameters = new List<IDataParameter>();
            if (!string.IsNullOrEmpty(Nom_prestation))
                parameters.Add(new MySqlParameter("@nom_prestation", MySqlDbType.VarChar) { Value = this.Nom_prestation });
            if (Id_type > 0)
                parameters.Add(new MySqlParameter("@id_type", MySqlDbType.Int32) { Value = this.Id_type });
            return parameters;
        }

        public override IBDDConnector getNewInstance() => new Prestation();
    }
}