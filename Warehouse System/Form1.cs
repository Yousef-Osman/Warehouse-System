using MetroFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warehouse_System
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        private Store store;
        private Item item;
        private Stakeholder stakeholder;
        private Permission permission;
        private Permission_Item permission_Item;

        #region Common Functions
        public Form1()
        {
            InitializeComponent();
            store = new Store();
            item = new Item();
            stakeholder = new Stakeholder();
            permission = new Permission();
            permission_Item = new Permission_Item();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            HideAllPanels();
            HomePanel.Visible = true;
        }
        private void LogoPanel_Click(object sender, EventArgs e)
        {
            TitleLabel.Text = "Home";
            HideAllPanels();
            HomePanel.Visible = true;
        }
        private void HideAllPanels()
        {
            HomePanel.Visible = false;
            StorePanel.Visible = false;
            ItemPanel.Visible = false;
            StakeholderPanel.Visible = false;
            PermissionsPanel.Visible = false;
            ReportsPanel.Visible = false;
            ReportReviewPanel.Visible = false;

            store.Id = item.Code = stakeholder.Id = permission.Id = 0;
        }
        #endregion

        #region Store EventHandlers
        private void StoreBtn_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            StorePanel.Visible = true;
            TitleLabel.Text = "Stores Details";
            ShowStoreData();
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(StoreNameTB.Text) && !string.IsNullOrWhiteSpace(StoreAddressTB.Text))
            {
                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    store.Name = StoreNameTB.Text;
                    store.Address = StoreAddressTB.Text;
                    store.Manager = StoreManagerTB.Text;

                    db.Entry(store).State = store.Id == 0 ? EntityState.Added : EntityState.Modified;
                    db.SaveChanges();
                }
                MetroMessageBox.Show(this, "New Store has been added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowStoreData();
                store.Id = 0;
            }
            else
            {
                Button btn = (Button)sender;
                if (btn.Name == "UpdateBtn")
                {
                    MetroMessageBox.Show(this, "Please fill the data to be updated or click on a row to update it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MetroMessageBox.Show(this, "Please fill the data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowStoreData()
        {
            using (WarehouseDBEntities db = new WarehouseDBEntities())
            {
                StoreDGV.DataSource = db.Stores.Select(s => new { s.Id, s.Name, s.Address, s.Manager }).ToList();
            }

            StoreNameTB.Text = StoreAddressTB.Text = StoreManagerTB.Text = "";
        }

        private void StoreDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (StoreDGV.CurrentRow.Index != -1)
            {
                store.Id = Convert.ToInt32(StoreDGV.CurrentRow.Cells["Id"].Value);
                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    store = db.Stores.Where(s => s.Id == store.Id).FirstOrDefault();

                    StoreNameTB.Text = store.Name;
                    StoreAddressTB.Text = store.Address;
                    StoreManagerTB.Text = store.Manager;
                }
            }
        }
        #endregion

        #region Item EventHandlers
        private void ItemBtn_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            ItemPanel.Visible = true;
            TitleLabel.Text = "Items Details";
            ItemCodeTB.Enabled = false;
            ShowItemData();
        }

        private void ItemAddBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ItemNameTB.Text) && !string.IsNullOrWhiteSpace(ItemUnitTB.Text))
            {
                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    item.Name = ItemNameTB.Text;
                    item.Unit = ItemUnitTB.Text;
                    item.ProductionDate = ItemPrdTP.Value;
                    item.ExpDate = ItemExpTP.Value;
                    item.SupplierId = Convert.ToInt32(ItemSpplierIdCB.SelectedItem);

                    db.Entry(item).State = item.Code == 0 ? EntityState.Added : EntityState.Modified;
                    db.SaveChanges();
                }
                string proccess = item.Code == 0 ? "Added" : "Modified";
                MetroMessageBox.Show(this, $"An Item has been {proccess}", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowItemData();
                item.Code = 0;
            }
            else
            {
                Button btn = (Button)sender;
                if (btn.Name.Contains("Update"))
                {
                    MetroMessageBox.Show(this, "Please fill the data to be updated or click on a row to update it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MetroMessageBox.Show(this, "Please fill the data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowItemData()
        {
            using (WarehouseDBEntities db = new WarehouseDBEntities())
            {
                ItemDGV.DataSource = db.Items.Select(i => new { i.Code, i.Name, i.Unit, i.ProductionDate, i.ExpDate, i.SupplierId }).ToList();

                ItemSpplierIdCB.DataSource = db.Stakeholders.Where(s => s.Role.Equals(1)).Select(s => s.Id).ToList();
                ItemSpplierIdCB.SelectedItem = null;
                ItemSpplierIdCB.SelectedText = "  -- Select Supplier ID --  ";
            }

            ItemNameTB.Text = ItemCodeTB.Text = ItemUnitTB.Text = "";
        }

        private void ItemDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ItemDGV.CurrentRow.Index != -1)
            {
                item.Code = Convert.ToInt32(ItemDGV.CurrentRow.Cells["Code"].Value);
                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    item = db.Items.Where(i => i.Code == item.Code).FirstOrDefault();

                    ItemCodeTB.Text = item.Code.ToString();
                    ItemCodeTB.Enabled = false;
                    ItemNameTB.Text = item.Name;
                    ItemUnitTB.Text = item.Unit;
                    ItemPrdTP.Value = item.ProductionDate;
                    ItemExpTP.Value = item.ExpDate;

                    ItemSpplierIdCB.DataSource = db.Stakeholders.Where(s => s.Role.Equals(1)).Select(s => s.Id).ToList();
                    ItemSpplierIdCB.SelectedItem = null;
                    ItemSpplierIdCB.SelectedText = item.SupplierId.ToString();
                }
            }
        }
        #endregion

        #region Stakeholders EventHandler
        private void StakeholderBtn_Click(object sender, EventArgs e)
        {
            HideAllPanels();

            Button btn = (Button)sender;
            int role;
            string title;

            if (btn.Name.Contains("Supplier"))
            {
                role = 1;
                title = "Supliers Details";
                CustomerBtnsPanel.Visible = false;
                SupplierBtnsPanel.Visible = true;
            }
            else
            {
                role = 2;
                title = "Customers Details";
                SupplierBtnsPanel.Visible = false;
                CustomerBtnsPanel.Visible = true;
            }

            TitleLabel.Text = title;
            StakeholderPanel.Visible = true;
            ShowStakeholdersData(role);
        }

        private void StakeholderAddBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(StakeholderNameTB.Text) && !string.IsNullOrWhiteSpace(SupplierPhoneTB.Text) && !string.IsNullOrWhiteSpace(SupplierEmailTB.Text))
            {
                Button btn = (Button)sender;
                int role = btn.Name.Contains("Supplier") ? 1 : 2;

                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    stakeholder.Name = StakeholderNameTB.Text;
                    stakeholder.Phone = SupplierPhoneTB.Text;
                    stakeholder.E_Mail = SupplierEmailTB.Text;
                    stakeholder.LandLine = SupplierLandLineTB.Text;
                    stakeholder.Fax = SupplierFaxTB.Text;
                    stakeholder.WebSite = SupplierWebsiteTB.Text;
                    stakeholder.Role = role;

                    db.Entry(stakeholder).State = stakeholder.Id == 0 ? EntityState.Added : EntityState.Modified;
                    db.SaveChanges();
                }

                string person = btn.Name.Contains("Supplier") ? "Supplier" : "Customer";
                string proccess = stakeholder.Id == 0 ? "Added" : "Modified";
                MetroMessageBox.Show(this, $"A {person} has been {proccess}", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowStakeholdersData(role);
                stakeholder.Id = 0;
            }
            else
            {
                Button btn = (Button)sender;
                if (btn.Name.Contains("Update"))
                {
                    MetroMessageBox.Show(this, "Please fill the data to be updated or click on a row to update it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MetroMessageBox.Show(this, "Please fill the data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowStakeholdersData(int role)
        {
            using (WarehouseDBEntities db = new WarehouseDBEntities())
            {
                SupplierDGV.DataSource = db.Stakeholders
                    .Where(s => s.Role.Equals(role))
                    .Select(s => new { s.Id, s.Name, s.Phone, s.E_Mail, s.LandLine, s.Fax, s.WebSite })
                    .ToList();
            }

            StakeholderNameTB.Text = SupplierPhoneTB.Text = SupplierEmailTB.Text = "";
            SupplierLandLineTB.Text = SupplierFaxTB.Text = SupplierWebsiteTB.Text = "";
        }

        private void SupplierDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (SupplierDGV.CurrentRow.Index != -1)
            {
                stakeholder.Id = Convert.ToInt32(SupplierDGV.CurrentRow.Cells["Id"].Value);
                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    stakeholder = db.Stakeholders.Where(s => s.Id == stakeholder.Id).FirstOrDefault();

                    StakeholderNameTB.Text = stakeholder.Name;
                    SupplierPhoneTB.Text = stakeholder.Phone;
                    SupplierEmailTB.Text = stakeholder.E_Mail;
                    SupplierLandLineTB.Text = stakeholder.LandLine;
                    SupplierFaxTB.Text = stakeholder.Fax;
                    SupplierWebsiteTB.Text = stakeholder.WebSite;
                }
            }
        }

        #endregion

        #region Permissions EventHandler
        private void PermissionsBtn_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            PermissionsPanel.Visible = true;
            TitleLabel.Text = "Permissions Details";
            ShowPermissionsData();
            PermissionDetailsPanel.Visible = false;
            PerItemsTabPanel.Visible = true;
        }

        private void ShowPermissionsData()
        {
            using (WarehouseDBEntities db = new WarehouseDBEntities())
            {
                ImportListDGV.DataSource = db.Permissions.Include(d => d.Store).Where(p => p.PermissionType.Equals(1)).Select(p => new { Permission_Number = p.Id, Store_Name = p.Store.Name, Permission_Date = p.PermissionDate }).ToList();

                ExportListDGV.DataSource = db.Permissions.Include(d => d.Store).Where(p => p.PermissionType.Equals(2)).Select(p => new { Permission_Number = p.Id, Store_Name = p.Store.Name, Permission_Date = p.PermissionDate }).ToList();

                ImportStoreIdCB.DataSource = ExportStoreIdCB.DataSource = db.Stores.Select(s => s.Id).ToList();
            }
            ImportStoreIdCB.SelectedItem = ExportStoreIdCB.SelectedItem = null;
            ImportStoreIdCB.SelectedText = ExportStoreIdCB.SelectedText = "        ------- Select Store ID -------      ";
        }

        /// <summary>
        /// Import Permission EventHandlers
        /// </summary>
        private void ImportAddBtn_Click(object sender, EventArgs e)
        {
            if (ImportStoreIdCB.Text != "        ------- Select Store ID -------      ")
            {
                Button btn = (Button)sender;

                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    permission.StoreId = Convert.ToInt32(ImportStoreIdCB.Text);
                    permission.PermissionDate = ImportDateTP.Value;
                    permission.PermissionType = 1;

                    db.Permissions.AddOrUpdate(permission);
                    db.SaveChanges();
                }

                string proccess = permission.Id == 0 ? "Added" : "Modified";
                MetroMessageBox.Show(this, $"A Permission has been {proccess}", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowPermissionsData();
                permission.Id = 0;
            }
            else
            {
                Button btn = (Button)sender;
                if (btn.Name.Contains("Update"))
                {
                    MetroMessageBox.Show(this, "Please fill the data to be updated or click on a row to update it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MetroMessageBox.Show(this, "Please fill the data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ImportListDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ImportListDGV.CurrentRow.Index != -1)
            {
                permission.Id = Convert.ToInt32(ImportListDGV.CurrentRow.Cells["Permission_Number"].Value);
                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    permission = db.Permissions.Where(p => p.Id == permission.Id).SingleOrDefault();

                    ImportPerIdTB.Text = permission.StoreId.ToString();
                    ImportDateTP.Value = permission.PermissionDate;

                    ImportStoreIdCB.DataSource = db.Stores.Select(s => s.Id).ToList();
                    ImportStoreIdCB.SelectedItem = null;
                    ImportStoreIdCB.SelectedText = permission.StoreId.ToString();
                }
            }
        }

        private void ImportListDGV_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int perNo = Convert.ToInt32(ImportListDGV.CurrentRow.Cells["Permission_Number"].Value);
            permission.Id = perNo;
            PermissionItemsLabel.Text = $"Items in Permission Number [{perNo}]";
            PerItemsTabPanel.Visible = false;
            PermissionDetailsPanel.Visible = true;
            ShowImportItemsDetails(perNo);
        }

        /// <summary>
        /// Import Permission EventHandlers
        /// </summary>
        private void EXportAddBtn_Click(object sender, EventArgs e)
        {
            if (ImportStoreIdCB.Text != "        ------- Select Store ID -------      ")
            {
                Button btn = (Button)sender;

                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    permission.StoreId = Convert.ToInt32(ExportStoreIdCB.Text);
                    permission.PermissionDate = ExportDateTP.Value;
                    permission.PermissionType = 2;

                    db.Permissions.AddOrUpdate(permission);
                    db.SaveChanges();
                }

                string proccess = permission.Id == 0 ? "Added" : "Modified";
                MetroMessageBox.Show(this, $"A Permission has been {proccess}", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowPermissionsData();
                permission.Id = 0;
            }
            else
            {
                Button btn = (Button)sender;
                if (btn.Name.Contains("Update"))
                {
                    MetroMessageBox.Show(this, "Please fill the data to be updated or click on a row to update it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MetroMessageBox.Show(this, "Please fill the data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EXportListDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ExportListDGV.CurrentRow.Index != -1)
            {
                permission.Id = Convert.ToInt32(ExportListDGV.CurrentRow.Cells["Permission_Number"].Value);
                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    permission = db.Permissions.Where(p => p.Id == permission.Id).SingleOrDefault();

                    ExportPerIdTB.Text = permission.StoreId.ToString();
                    ExportDateTP.Value = permission.PermissionDate;

                    ExportStoreIdCB.DataSource = db.Stores.Select(s => s.Id).ToList();
                    ExportStoreIdCB.SelectedItem = null;
                    ExportStoreIdCB.SelectedText = permission.StoreId.ToString();
                }
            }
        }

        private void EXportListDGV_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int perNo = Convert.ToInt32(ExportListDGV.CurrentRow.Cells["Permission_Number"].Value);
            permission.Id = perNo;
            PermissionItemsLabel.Text = $"Items in Permission Number [{perNo}]";
            PerItemsTabPanel.Visible = false;
            PermissionDetailsPanel.Visible = true;
            ShowImportItemsDetails(perNo);
        }

        /// <summary>
        /// Event handlers for the items inside each permission
        /// </summary>
        private void ShowImportItemsDetails(int permissionNumber)
        {
            using (WarehouseDBEntities db = new WarehouseDBEntities())
            {
                PermissionItemsDGV.DataSource = (from p in db.Permissions
                                                 join p_i in db.Permission_Item on p.Id equals p_i.PermissionId
                                                 join i in db.Items on p_i.ItemCode equals i.Code
                                                 join s in db.Stakeholders on i.SupplierId equals s.Id
                                                 where p.Id == permissionNumber
                                                 select new
                                                 {
                                                     Item_Code = i.Code,
                                                     Item_Name = i.Name,
                                                     Production_Date = i.ProductionDate,
                                                     Expiration_Date = i.ExpDate,
                                                     Supplier_Name = s.Name,
                                                     p_i.Quantity
                                                 }).ToList();

                PermissionItemCodeCB.DataSource = db.Items.Select(s => s.Code).ToList();
            }
            PermissionIdTB.Text = permission.Id.ToString();
            PermissionItemCodeCB.SelectedItem = null;
            PermissionItemCodeCB.SelectedText = "      ------- Select Item Code -------    ";
        }

        private void ImportAddItemBtn_Click(object sender, EventArgs e)
        {
            if (PermissionItemQtyIUD.Value != 0 && PermissionItemCodeCB.Text != "      ------- Select Item Code -------    ")
            {
                Button btn = (Button)sender;

                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    permission_Item.PermissionId = Convert.ToInt32(PermissionIdTB.Text);
                    permission_Item.ItemCode = Convert.ToInt32(PermissionItemCodeCB.Text);
                    permission_Item.Quantity = Convert.ToInt32(PermissionItemQtyIUD.Value);

                    db.Permission_Item.AddOrUpdate(permission_Item);
                    db.SaveChanges();
                }

                string proccess = stakeholder.Id == 0 ? "Added" : "Modified";
                MetroMessageBox.Show(this, $"An Item has been {proccess}", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowImportItemsDetails(permission_Item.PermissionId);

            }
            else
            {
                Button btn = (Button)sender;
                if (btn.Name.Contains("Update"))
                {
                    MetroMessageBox.Show(this, "Please fill the data to be updated or click on a row to update it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MetroMessageBox.Show(this, "Please fill the data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ImportItemsDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ImportListDGV.CurrentRow.Index != -1)
            {
                permission_Item.PermissionId = Convert.ToInt32(PermissionIdTB.Text);
                PermissionItemCodeCB.Text = (PermissionItemsDGV.CurrentRow.Cells["Item_Code"].Value).ToString();
                PermissionItemQtyIUD.Value = Convert.ToInt32(PermissionItemsDGV.CurrentRow.Cells["Quantity"].Value);
            }
        }

        private void ImportBackBtn_Click(object sender, EventArgs e)
        {
            PermissionDetailsPanel.Visible = false;
            PerItemsTabPanel.Visible = true;
            permission.Id = 0;
            ShowPermissionsData();
        }
        #endregion

        #region Reports EventHandlers
        private void ReportsBtn_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            TitleLabel.Text = "Reports";
            ReportsPanel.Visible = true;
        }

        private void ReportReviewBtn_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            ReportReviewPanel.Visible = true;
        }
        #endregion
    }
}
