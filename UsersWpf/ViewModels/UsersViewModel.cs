using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersClassLibrary.Controllers;
using UsersClassLibrary.Models;

namespace UsersWpf.ViewModels
{
    internal class UsersViewModel : BaseViewModel
    {
		private string _filtraNome;

		public string FiltraNome
		{
			get { return _filtraNome; }
			set { _filtraNome = value; NotifyPropertyChanged("FiltraNome"); LoadData(); }
		}

		private ObservableCollection<string> _listaSessi;

		public ObservableCollection<string> ListaSessi
		{
			get { return _listaSessi; }
			set { _listaSessi = value; NotifyPropertyChanged("ListaSessi"); }
		}

		private string _filtraSesso;

		public string FiltraSesso
		{
			get { return _filtraSesso; }
			set { _filtraSesso = value; NotifyPropertyChanged("FiltraSesso"); LoadData(); }
		}

		private List<User> _datiUtenti;

		public List<User> DatiUtenti
		{
			get { return _datiUtenti; }
			set { _datiUtenti = value; NotifyPropertyChanged("DatiUtenti"); }
		}

		private User _utenteSelezionato;

		public User UtenteSelezionato
		{
			get { return _utenteSelezionato; }
			set { _utenteSelezionato = value; NotifyPropertyChanged("UtenteSelezionato"); }
		}

		public UsersViewModel()
		{
			ListaSessi = new ObservableCollection<string>(Users.GetGenders());
			DatiUtenti = Users.GetAll();
		}

		private void LoadData()
		{
			DatiUtenti = Users.FindAll(FiltraNome, FiltraSesso);
				/*(u =>
				( string.IsNullOrEmpty(FiltraNome) || u.FirstName.Contains(FiltraNome) || u.LastName.Contains(FiltraNome) ) &&
				( string.IsNullOrEmpty(FiltraSesso) || u.Gender == FiltraSesso )*/
			
		}

		public void AzzeraFiltri()
		{
			FiltraNome = "";
			FiltraSesso = null;
		}

        public void NewUser()
        {
            EditUserView editUser = new EditUserView();
            editUser.ShowDialog();
			LoadData();
        }

		public void EditUser()
		{
            EditUserView editUser = new EditUserView(UtenteSelezionato);
            editUser.ShowDialog();
            LoadData();
        }
	}
}
