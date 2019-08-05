using Syncfusion.DataSource.Extensions;
using Syncfusion.ListView.XForms;
using Syncfusion.ListView.XForms.Control.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using SelectionMode = Syncfusion.ListView.XForms.SelectionMode;

namespace CustomSelection
{
   public class Behavior : Behavior<ContentPage>
    {
        #region Fields
        public SelectionViewModel ViewModel { get; set; }
        private TapGestureRecognizer editImageTapped;
        private TapGestureRecognizer cancelImageTapped;
        SfListView ListView;Grid HeaderGrid; Image SelectionEdit, SelectionCancel;

        #endregion
        protected override void OnAttachedTo(ContentPage bindable)
        {
            ListView = bindable.FindByName<SfListView>("listView");
            HeaderGrid = bindable.FindByName<Grid>("headerGrid");
            SelectionEdit= bindable.FindByName<Image>("selectionEditImage");
            SelectionCancel=bindable.FindByName<Image>("selectionCancelImage");
            ViewModel = new SelectionViewModel();
            ListView.BindingContext = ViewModel;
            ListView.ItemsSource = ViewModel.MusicInfo;

            ListView.DataSource.GroupDescriptors.Add(new Syncfusion.DataSource.GroupDescriptor()
            {
                PropertyName = "SongTitle",
                KeySelector = (object obj1) =>
                {
                    var item = (obj1 as MusicInfo);
                    return item.SongTitle[0].ToString();
                }
            });


            ListView.ItemHolding += ListView_ItemHolding;
            ListView.SelectionChanged += ListView_OnSelectionChanged;
            HeaderGrid.BindingContext = ViewModel;

            editImageTapped = new TapGestureRecognizer() { Command = new Command(EditImageTapped) };
            this.SelectionEdit.GestureRecognizers.Add(editImageTapped);

            cancelImageTapped = new TapGestureRecognizer() { Command = new Command(CancelImageTapped) };
            this.SelectionCancel.GestureRecognizers.Add(cancelImageTapped);

            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            base.OnDetachingFrom(bindable);
        }

        #region Private Methods

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var image = (sender as Image);
            var groupResult = image.BindingContext as GroupResult;

            if (groupResult == null)
                return;

            var items = groupResult.Items.ToList<MusicInfo>().ToList();

            if ((items.All(listItem => listItem.IsSelected == true)))
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    var item = items[i];
                    (item as MusicInfo).IsSelected = false;
                }
            }
            else if ((items.All(listItem => listItem.IsSelected == false)))
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    var item = items[i];
                    (item as MusicInfo).IsSelected = true;
                }
            }

            this.RefreshGroupHeader(groupResult);
            ListView.RefreshView();
        }

        private void RefreshGroupHeader(GroupResult group)
        {
            foreach (var item in this.ListView.GetVisualContainer().Children)
            {
                if (item.BindingContext == group)
                {
                    item.BindingContext = null;
                    (item as GroupHeaderItem).Content.BindingContext = null;
                }
            }
        }

        private void EditImageTapped()
        {
            this.ListView.SelectedItems.Clear();
            this.ListView.RefreshView();
            UpdateSelectionTempate();
        }

        private void CancelImageTapped()
        {
            GroupResult group = null;

            for (int i = 0; i < ListView.SelectedItems.Count; i++)
            {
                var item = ListView.SelectedItems[i] as MusicInfo;
                item.IsSelected = false;

                var temp = GetGroup(item);

                if (group != temp)
                {
                    RefreshGroupHeader(temp);
                    group = temp;
                }
            }

            this.ListView.SelectedItems.Clear();
            UpdateSelectionTempate();
            this.ListView.RefreshView();
        }

        private void UpdateSelectionTempate()
        {
            if (ListView.SelectedItems.Count > 0 || SelectionEdit.IsVisible)
            {
                ListView.SelectionMode = SelectionMode.Multiple;
                ListView.SelectionBackgroundColor = Color.Transparent;
                ListView.SelectedItems.Clear();
                ViewModel.TitleInfo = "";
                ViewModel.IsVisible = true;
                SelectionEdit.IsVisible = false;
            }
            else
            {
                ListView.SelectionMode = SelectionMode.Single;
                ListView.SelectionBackgroundColor = Color.FromRgb(228, 228, 228);
                ViewModel.HeaderInfo = "";
                ViewModel.TitleInfo = "Music Library";
                ViewModel.IsVisible = false;
                SelectionEdit.IsVisible = true;
            }
        }

        #endregion

        #region CallBacks

        private void ListView_ItemHolding(object sender, ItemHoldingEventArgs e)
        {
            if (ListView.SelectionMode != SelectionMode.Multiple)
                UpdateSelectionTempate();
        }


        private void ListView_OnSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if (ListView.SelectionMode == SelectionMode.Multiple)
            {
                GroupResult group = null;

                for (int i = 0; i < e.AddedItems.Count; i++)
                {
                    var item = e.AddedItems[i];
                    (item as MusicInfo).IsSelected = true;
                    group = GetGroup(item);
                }
                for (int i = 0; i < e.RemovedItems.Count; i++)
                {
                    var item = e.RemovedItems[i];
                    (item as MusicInfo).IsSelected = false;
                    group = GetGroup(item);
                }

                if (group != null)
                    RefreshGroupHeader(group);

                ListView.RefreshView();
            }
        }

        private GroupResult GetGroup(object itemData)
        {
            GroupResult itemGroup = null;

            foreach (var item in this.ListView.DataSource.DisplayItems)
            {
                if (item is GroupResult)
                {
                    itemGroup = item as GroupResult;
                }
                if (item == itemData)
                    break;
            }
            return itemGroup;
        }

        #endregion

    }
}
