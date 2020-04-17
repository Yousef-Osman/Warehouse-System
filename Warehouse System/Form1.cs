using MetroFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
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
        private Stakeholder supplier;
        private Permission permission;
        private Permission_Item permission_Item;

        #region Common Functions
        public Form1()
        {
            InitializeComponent();
            store = new Store();
            item = new Item();
            supplier = new Stakeholder();
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
                    supplier.Name = StakeholderNameTB.Text;
                    supplier.Phone = SupplierPhoneTB.Text;
                    supplier.E_Mail = SupplierEmailTB.Text;
                    supplier.LandLine = SupplierLandLineTB.Text;
                    supplier.Fax = SupplierFaxTB.Text;
                    supplier.WebSite = SupplierWebsiteTB.Text;
                    supplier.Role = role;

                    db.Entry(supplier).State = supplier.Id == 0 ? EntityState.Added : EntityState.Modified;
                    db.SaveChanges();
                }

                string person = btn.Name.Contains("Supplier") ? "Supplier" : "Customer";
                string proccess = supplier.Id == 0 ? "Added" : "Modified";
                MetroMessageBox.Show(this, $"A {person} has been {proccess}", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowStakeholdersData(role);
                supplier.Id = 0;
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
                supplier.Id = Convert.ToInt32(SupplierDGV.CurrentRow.Cells["Id"].Value);
                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    supplier = db.Stakeholders.Where(s => s.Id == supplier.Id).FirstOrDefault();

                    StakeholderNameTB.Text = supplier.Name;
                    SupplierPhoneTB.Text = supplier.Phone;
                    SupplierEmailTB.Text = supplier.E_Mail;
                    SupplierLandLineTB.Text = supplier.LandLine;
                    SupplierFaxTB.Text = supplier.Fax;
                    SupplierWebsiteTB.Text = supplier.WebSite;
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
            ShowImportData(1);
            ImportDetailsPanel.Visible = false;
            PerItemsTabPanel.Visible = true;
        }

        private void ShowImportData(int type)
        {
            using (WarehouseDBEntities db = new WarehouseDBEntities())
            {
                ImportListDGV.DataSource = db.Permissions.Include(d=>d.Store).Where(p => p.PermissionType.Equals(type)).Select(p => new { Permission_Number = p.Id, Store_Name = p.Store.Name, Permission_Date = p.PermissionDate }).ToList();

                //ImportListDGV.DataSource = (from p in db.Permissions
                //                            join p_i in db.Permission_Item on p.Id equals p_i.PermissionId
                //                            join i in db.Items on p_i.ItemCode equals i.Code
                //                            where p.PermissionType == type
                //                            select new { 
                //                                PermissionId = p.Id,
                //                                storeName = i.Name,
                //                                p.PermissionDate
                //                            }).ToList();

                ImportStoreIdCB.DataSource = db.Stores.Select(s => s.Id).ToList();
            }
            ImportStoreIdCB.SelectedItem = null;
            ImportStoreIdCB.SelectedText = "        ------- Select Store ID -------      ";
        }

        private void ImportAddBtn_Click(object sender, EventArgs e)
        {

        }

        private void ShowImportItemsDetails()
        {
            using (WarehouseDBEntities db = new WarehouseDBEntities())
            {
                ImportItemsDGV.DataSource = (from p in db.Permissions
                                             join p_i in db.Permission_Item on p.Id equals p_i.PermissionId
                                             join i in db.Items on p_i.ItemCode equals i.Code
                                             join s in db.Stakeholders on i.SupplierId equals s.Id
                                             select new
                                             {
                                                 Item_Code = i.Code,
                                                 Item_Name = i.Name,
                                                 Production_Date = i.ProductionDate,
                                                 Expiration_Date = i.ExpDate,
                                                 Supplier_Name = s.Name,
                                                 p_i.Quantity
                                             }).ToList();

                ImportItemCodeCB.DataSource = db.Items.Select(s => s.Code).ToList();
            }
            ImportPermissionIdTB.Text = permission.Id.ToString();
            ImportItemCodeCB.SelectedItem = null;
            ImportItemCodeCB.SelectedText = "      ------- Select Item Code -------    ";
        }

        private void ImportBackBtn_Click(object sender, EventArgs e)
        {
            ImportDetailsPanel.Visible = false;
            PerItemsTabPanel.Visible = true;
            permission.Id = 0;
            ShowImportData(1);
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

        private void ImportAddItemBtn_Click(object sender, EventArgs e)
        {
            if (ImportItemQtyIUD.Value != 0 && ImportItemCodeCB.Text != "      ------- Select Item Code -------    ")
            {
                Button btn = (Button)sender;

                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    permission_Item.PermissionId = Convert.ToInt32(ImportPermissionIdTB.Text);
                    permission_Item.ItemCode = Convert.ToInt32(ImportItemCodeCB.Text);
                    permission_Item.Quantity = Convert.ToInt32(ImportItemQtyIUD.Value);

                    var item = db.Permission_Item.Where(a => a.PermissionId == permission_Item.PermissionId && a.ItemCode == permission_Item.ItemCode).FirstOrDefault();

                    bool exists = item != null ? true : false;

                    db.Entry(permission_Item).State = exists == false ? EntityState.Added : EntityState.Modified;
                    db.SaveChanges();
                }

                string proccess = supplier.Id == 0 ? "Added" : "Modified";
                MetroMessageBox.Show(this, $"An Item has been {proccess}", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowImportItemsDetails();
                
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
            int PerNo = Convert.ToInt32(ImportListDGV.CurrentRow.Cells["Permission_Number"].Value);
            permission.Id = PerNo;
            ImportPermissionItemsLabel.Text = $"Items in Permission Number [{PerNo}]";
            PerItemsTabPanel.Visible = false;
            ImportDetailsPanel.Visible = true;
            ShowImportItemsDetails();
        }
    }
}
