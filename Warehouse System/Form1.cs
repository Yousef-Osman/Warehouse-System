﻿using MetroFramework;
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
        private Stakeholder customer;
        //public enum shareHolderType1 = {supplier = 1; customer = 2;};

        public Form1()
        {
            InitializeComponent();
            store = new Store();
            item = new Item();
            supplier = new Stakeholder();
            customer = new Stakeholder();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HideAllPanels();
            HomePanel.Visible = true;
        }

        private void LogoPanel_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            HomePanel.Visible = true;
        }

        #region Store EventHandlers
        private void StoreBtn_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            StorePanel.Visible = true;
            TitleLabel.Text = "Store Details";
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
                    store.ManagerId = Convert.ToInt32(StoreManagerCB.SelectedItem);

                    db.Entry(store).State = store.Id == 0 ? EntityState.Added : EntityState.Modified;
                    db.SaveChanges();
                }
                MetroMessageBox.Show(this, "New Store has been added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowStoreData();
            }
            else
            {
                Button btn = (Button)sender;
                if (btn.Name == "UpdateBtn")
                {
                    MetroMessageBox.Show(this, "Please fill the data to be updated or double click a row to update it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                StoreDGV.DataSource = db.Stores.Include(a => a.Manger).Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Address,
                    Manger = s.Manger.Name
                }).ToList();
                StoreManagerCB.DataSource = db.Mangers.Select(a => a.Id).ToList();
                StoreManagerCB.SelectedItem = null;
                StoreManagerCB.SelectedText = "  -------- Select Manager ID --------  ";
            }

            StoreNameTB.Text = "";
            StoreAddressTB.Text = "";
        }

        private void StoreDGV_DoubleClick(object sender, EventArgs e)
        {
            if (StoreDGV.CurrentRow.Index != -1)
            {
                store.Id = Convert.ToInt32(StoreDGV.CurrentRow.Cells["Id"].Value);
                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    store = db.Stores.Where(s => s.Id == store.Id).FirstOrDefault();

                    StoreNameTB.Text = store.Name;
                    StoreAddressTB.Text = store.Address;
                    StoreManagerCB.DataSource = db.Mangers.Select(a => a.Id).ToList();
                    StoreManagerCB.SelectedItem = null;
                    StoreManagerCB.SelectedText = store.ManagerId.ToString();
                }
            }
        }
        #endregion

        #region Item EventHandlers
        private void ItemBtn_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            ItemPanel.Visible = true;
            TitleLabel.Text = "Item Details";
            ShowItemData();
        }

        private void ItemAddBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(StoreNameTB.Text) && !string.IsNullOrWhiteSpace(StoreAddressTB.Text))
            {
                using (WarehouseDBEntities db = new WarehouseDBEntities())
                {
                    store.Name = StoreNameTB.Text;
                    store.Address = StoreAddressTB.Text;
                    store.ManagerId = Convert.ToInt32(StoreManagerCB.SelectedItem);

                    db.Entry(store).State = store.Id == 0 ? EntityState.Added : EntityState.Modified;
                    db.SaveChanges();
                }
                MetroMessageBox.Show(this, "New Store has been added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowStoreData();
                item.Id = 0;
            }
            else
            {
                Button btn = (Button)sender;
                if (btn.Name == "UpdateBtn")
                {
                    MetroMessageBox.Show(this, "Please fill the data to be updated or double click a row to update it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                ItemSpplierIdCB.SelectedText = "  ----- Select Manager ID -----  ";
            }

            ItemNameTB.Text = ItemCodeTB.Text = ItemUnitTB.Text= "";
        }
        #endregion

        private void HideAllPanels()
        {
            HomePanel.Visible = false;
            StorePanel.Visible = false;
            ItemPanel.Visible = false;
            StakeholderPanel.Visible = false;
        }

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
                title = "Suplier Details";
                CustomerBtnsPanel.Visible = false;
                SupplierBtnsPanel.Visible = true;
            }
            else
            {
                role = 2;
                title = "Customer Details";
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
                MetroMessageBox.Show(this, $"New {person} has been added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowStakeholdersData(role);
                supplier.Id = 0;
            }
            else
            {
                Button btn = (Button)sender;
                if (btn.Name.Contains("Update"))
                {
                    MetroMessageBox.Show(this, "Please fill the data to be updated or double click a row to update it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        //#region Customer EventHandler
        //private void SupplierBtn_Click(object sender, EventArgs e)
        //{
        //    HideAllPanels();
        //    SupplierPanel.Visible = true;
        //    ShowStakeholdersData();
        //}

        //private void SupplierAddBtn_Click(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrWhiteSpace(SupplierNameTB.Text) && !string.IsNullOrWhiteSpace(SupplierPhoneTB.Text) && !string.IsNullOrWhiteSpace(SupplierEmailTB.Text))
        //    {
        //        using (WarehouseDBEntities db = new WarehouseDBEntities())
        //        {
        //            supplier.Name = SupplierNameTB.Text;
        //            supplier.Phone = SupplierPhoneTB.Text;
        //            supplier.E_Mail = SupplierEmailTB.Text;
        //            supplier.LandLine = SupplierLandLineTB.Text;
        //            supplier.Fax = SupplierFaxTB.Text;
        //            supplier.WebSite = SupplierWebsiteTB.Text;
        //            supplier.Role = 1;

        //            db.Entry(supplier).State = supplier.Id == 0 ? EntityState.Added : EntityState.Modified;
        //            db.SaveChanges();
        //        }
        //        MetroMessageBox.Show(this, "New Supplier has been added", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        ShowStakeholdersData();
        //        supplier.Id = 0;
        //    }
        //    else
        //    {
        //        Button btn = (Button)sender;
        //        if (btn.Name == "SupplierUpdateBtn")
        //        {
        //            MetroMessageBox.Show(this, "Please fill the data to be updated or double click a row to update it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //        else
        //        {
        //            MetroMessageBox.Show(this, "Please fill the data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}

        //private void ShowStakeholdersData()
        //{
        //    using (WarehouseDBEntities db = new WarehouseDBEntities())
        //    {
        //        SupplierDGV.DataSource = db.Stakeholders
        //            .Where(s => s.Role.Equals(1))
        //            .Select(s => new { s.Id, s.Name, s.Phone, s.E_Mail, s.LandLine, s.Fax, s.WebSite })
        //            .ToList();
        //    }

        //    SupplierNameTB.Text = SupplierPhoneTB.Text = SupplierEmailTB.Text = "";
        //    SupplierLandLineTB.Text = SupplierFaxTB.Text = SupplierWebsiteTB.Text = "";
        //}

        //private void SupplierDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (SupplierDGV.CurrentRow.Index != -1)
        //    {
        //        supplier.Id = Convert.ToInt32(SupplierDGV.CurrentRow.Cells["Id"].Value);
        //        using (WarehouseDBEntities db = new WarehouseDBEntities())
        //        {
        //            supplier = db.Stakeholders.Where(s => s.Id == supplier.Id).FirstOrDefault();

        //            SupplierNameTB.Text = supplier.Name;
        //            SupplierPhoneTB.Text = supplier.Phone;
        //            SupplierEmailTB.Text = supplier.E_Mail;
        //            SupplierLandLineTB.Text = supplier.LandLine;
        //            SupplierFaxTB.Text = supplier.Fax;
        //            SupplierWebsiteTB.Text = supplier.WebSite;
        //        }
        //    }
        //}
        //#endregion
    }
}
