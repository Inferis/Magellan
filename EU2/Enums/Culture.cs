using System;
using System.Drawing;

namespace EU2.Enums {
	/// <summary>
	/// Summary description for Culture.
	/// </summary>
	public class Culture {
		string name;

		public Culture() {
			name = "none";
		}

		public Culture( string name ) {
			this.name = name;
		}

		public string Name {
			get { return name; }
		}

		public override string ToString() {
			return name;
		}

		#region All Cultures
		static public Culture None 			{ get { return none; } }
		static public Culture Abenaki 		{ get { return abenaki; } }
		static public Culture Aborigin 		{ get { return aborigin; } }
		static public Culture Afghani 		{ get { return afghani; } }
		static public Culture Aka 			{ get { return aka; } }
		static public Culture Albanian 		{ get { return albanian; } }
		static public Culture Aleutian 		{ get { return aleutian; } }
		static public Culture Amazonian 	{ get { return amazonian; } }
		static public Culture Andean 		{ get { return andean; } }
		static public Culture Arabic 		{ get { return arabic; } }
		static public Culture Armenian 		{ get { return armenian; } }
		static public Culture Ashanti 		{ get { return ashanti; } }
		static public Culture Aztek 		{ get { return aztek; } }
		static public Culture Altai 		{ get { return altai; } }
		static public Culture Anglosaxon 	{ get { return anglosaxon; } }
		static public Culture Baltic 		{ get { return baltic; } }
		static public Culture Baluchi 		{ get { return baluchi; } }
		static public Culture Bantu 		{ get { return bantu; } }
		static public Culture Basque 		{ get { return basque; } }
		static public Culture Bengali 		{ get { return bengali; } }
		static public Culture Berber 		{ get { return berber; } }
		static public Culture Burmanese 	{ get { return burmanese; } }
		static public Culture Cambodian 	{ get { return cambodian; } }
		static public Culture Canary 		{ get { return canary; } }
		static public Culture Cantonese 	{ get { return cantonese; } }
		static public Culture Caribbean 	{ get { return caribbean; } }
		static public Culture Cherokee 		{ get { return cherokee; } }
		static public Culture Cree 			{ get { return cree; } }
		static public Culture Creek 		{ get { return creek; } }
		static public Culture Czech 		{ get { return czech; } }
		static public Culture Dravidian 	{ get { return dravidian; } }
		static public Culture Dakota 		{ get { return dakota; } }
		static public Culture Delaware 		{ get { return delaware; } }
		static public Culture Dutch 		{ get { return dutch; } }
		static public Culture Dyola 		{ get { return dyola; } }
		static public Culture Ethiopian 	{ get { return ethiopian; } }
		static public Culture Filippine 	{ get { return filippine; } }
		static public Culture French 		{ get { return french; } }
		static public Culture Gaelic 		{ get { return gaelic; } }
		static public Culture Georgian 		{ get { return georgian; } }
		static public Culture German 		{ get { return german; } }
		static public Culture Greek 		{ get { return greek; } }
		static public Culture Guajiro 		{ get { return guajiro; } }
		static public Culture Gujarati 		{ get { return gujarati; } }
		static public Culture Han 			{ get { return han; } }
		static public Culture Hawaiian 		{ get { return hawaiian; } }
		static public Culture Hindi 		{ get { return hindi; } }
		static public Culture Huron 		{ get { return huron; } }
		static public Culture Iberian 		{ get { return iberian; } }
		static public Culture Indian 		{ get { return indian; } }
		static public Culture Indonesian 	{ get { return indonesian; } }
		static public Culture Inuit 		{ get { return inuit; } }
		static public Culture Italian 		{ get { return italian; } }
		static public Culture Iroquis 		{ get { return iroquis; } }
		static public Culture Japanese 		{ get { return japanese; } }
		static public Culture Javan 		{ get { return javan; } }
		static public Culture Kongolese 	{ get { return kongolese; } }
		static public Culture Khazak 		{ get { return khazak; } }
		static public Culture Khmer 		{ get { return khmer; } }
		static public Culture Korean 		{ get { return korean; } }
		static public Culture Kurdish 		{ get { return kurdish; } }
		static public Culture Laotian 		{ get { return laotian; } }
		static public Culture Lithuanian 	{ get { return lithuanian; } }
		static public Culture Madagasque 	{ get { return madagasque; } }
		static public Culture Magyar 		{ get { return magyar; } }
		static public Culture Malay 		{ get { return malay; } }
		static public Culture Mali 			{ get { return mali; } }
		static public Culture Maltese 		{ get { return maltese; } }
		static public Culture Mayan 		{ get { return mayan; } }
		static public Culture Melanese 		{ get { return melanese; } }
		static public Culture Manchu 		{ get { return manchu; } }
		static public Culture Marathi 		{ get { return marathi; } }
		static public Culture Mataco 		{ get { return mataco; } }
		static public Culture Mesoamerican 	{ get { return mesoamerican; } }
		static public Culture Mississippian { get { return mississippian; } }
		static public Culture Mongol 		{ get { return mongol; } }
		static public Culture Naskapi 		{ get { return naskapi; } }
		static public Culture Navajo 		{ get { return navajo; } }
		static public Culture Nubian 		{ get { return nubian; } }
		static public Culture Patagonian 	{ get { return patagonian; } }
		static public Culture Persian 		{ get { return persian; } }
		static public Culture Polish 		{ get { return polish; } }
		static public Culture Polynese 		{ get { return polynese; } }
		static public Culture Russian 		{ get { return russian; } }
		static public Culture Romanian 		{ get { return romanian; } }
		static public Culture Ruthenian 	{ get { return ruthenian; } }
		static public Culture Scandinavian 	{ get { return scandinavian; } }
		static public Culture Senegambian 	{ get { return senegambian; } }
		static public Culture Shawnee 		{ get { return shawnee; } }
		static public Culture Shona 		{ get { return shona; } }
		static public Culture Sikh 			{ get { return sikh; } }
		static public Culture Slavonic 		{ get { return slavonic; } }
		static public Culture Slovak 		{ get { return slovak; } }
		static public Culture Somali 		{ get { return somali; } }
		static public Culture Swahili 		{ get { return swahili; } }
		static public Culture Swiss 		{ get { return swiss; } }
		static public Culture Syrian 		{ get { return syrian; } }
		static public Culture Teremembe 	{ get { return teremembe; } }
		static public Culture Thai 			{ get { return thai; } }
		static public Culture Tibetan 		{ get { return tibetan; } }
		static public Culture Tuareg 		{ get { return tuareg; } }
		static public Culture Tupinamba 	{ get { return tupinamba; } }
		static public Culture Turkish 		{ get { return turkish; } }
		static public Culture Ugric 		{ get { return ugric; } }
		static public Culture Ukrainian 	{ get { return ukrainian; } }
		static public Culture Uzbehk 		{ get { return uzbehk; } }
		static public Culture Vietnamese 	{ get { return vietnamese; } }
		static public Culture Yorumba 		{ get { return yorumba; } }
		static public Culture Zapotek 		{ get { return zapotek; } }
		#endregion

		#region private values
		static private Culture none = new Culture();
		static private Culture abenaki = new Culture( "abenaki" );
		static private Culture aborigin = new Culture( "aborigin" );
		static private Culture afghani = new Culture( "afghani" );
		static private Culture aka = new Culture( "aka" );
		static private Culture albanian = new Culture( "albanian" );
		static private Culture aleutian = new Culture( "aleutian" );
		static private Culture amazonian = new Culture( "amazonian" );
		static private Culture andean = new Culture( "andean" );
		static private Culture arabic = new Culture( "arabic" );
		static private Culture armenian = new Culture( "armenian" );
		static private Culture ashanti = new Culture( "ashanti" );
		static private Culture aztek = new Culture( "aztek" );
		static private Culture altai = new Culture( "altai" );
		static private Culture anglosaxon = new Culture( "anglosaxon" );
		static private Culture baltic = new Culture( "baltic" );
		static private Culture baluchi = new Culture( "baluchi" );
		static private Culture bantu = new Culture( "bantu" );
		static private Culture basque = new Culture( "basque" );
		static private Culture bengali = new Culture( "bengali" );
		static private Culture berber = new Culture( "berber" );
		static private Culture burmanese = new Culture( "burmanese" );
		static private Culture cambodian = new Culture( "cambodian" );
		static private Culture canary = new Culture( "canary" );
		static private Culture cantonese = new Culture( "cantonese" );
		static private Culture caribbean = new Culture( "caribbean" );
		static private Culture cherokee = new Culture( "cherokee" );
		static private Culture cree = new Culture( "cree" );
		static private Culture creek = new Culture( "creek" );
		static private Culture czech = new Culture( "czech" );
		static private Culture dravidian = new Culture( "dravidian" );
		static private Culture dakota = new Culture( "dakota" );
		static private Culture delaware = new Culture( "delaware" );
		static private Culture dutch = new Culture( "dutch" );
		static private Culture dyola = new Culture( "dyola" );
		static private Culture ethiopian = new Culture( "ethiopian" );
		static private Culture filippine = new Culture( "filippine" );
		static private Culture french = new Culture( "french" );
		static private Culture gaelic = new Culture( "gaelic" );
		static private Culture georgian = new Culture( "georgian" );
		static private Culture german = new Culture( "german" );
		static private Culture greek = new Culture( "greek" );
		static private Culture guajiro = new Culture( "guajiro" );
		static private Culture gujarati = new Culture( "gujarati" );
		static private Culture han = new Culture( "han" );
		static private Culture hawaiian = new Culture( "hawaiian" );
		static private Culture hindi = new Culture( "hindi" );
		static private Culture huron = new Culture( "huron" );
		static private Culture iberian = new Culture( "iberian" );
		static private Culture indian = new Culture( "indian" );
		static private Culture indonesian = new Culture( "indonesian" );
		static private Culture inuit = new Culture( "inuit" );
		static private Culture italian = new Culture( "italian" );
		static private Culture iroquis = new Culture( "iroquis" );
		static private Culture japanese = new Culture( "japanese" );
		static private Culture javan = new Culture( "javan" );
		static private Culture kongolese = new Culture( "kongolese" );
		static private Culture khazak = new Culture( "khazak" );
		static private Culture khmer = new Culture( "khmer" );
		static private Culture korean = new Culture( "korean" );
		static private Culture kurdish = new Culture( "kurdish" );
		static private Culture laotian = new Culture( "laotian" );
		static private Culture lithuanian = new Culture( "lithuanian" );
		static private Culture madagasque = new Culture( "madagasque" );
		static private Culture magyar = new Culture( "magyar" );
		static private Culture malay = new Culture( "malay" );
		static private Culture mali = new Culture( "mali" );
		static private Culture maltese = new Culture( "maltese" );
		static private Culture mayan = new Culture( "mayan" );
		static private Culture melanese = new Culture( "melanese" );
		static private Culture manchu = new Culture( "manchu" );
		static private Culture marathi = new Culture( "marathi" );
		static private Culture mataco = new Culture( "mataco" );
		static private Culture mesoamerican = new Culture( "mesoamerican" );
		static private Culture mississippian = new Culture( "mississippian" );
		static private Culture mongol = new Culture( "mongol" );
		static private Culture naskapi = new Culture( "naskapi" );
		static private Culture navajo = new Culture( "navajo" );
		static private Culture nubian = new Culture( "nubian" );
		static private Culture patagonian = new Culture( "patagonian" );
		static private Culture persian = new Culture( "persian" );
		static private Culture polish = new Culture( "polish" );
		static private Culture polynese = new Culture( "polynese" );
		static private Culture russian = new Culture( "russian" );
		static private Culture romanian = new Culture( "romanian" );
		static private Culture ruthenian = new Culture( "ruthenian" );
		static private Culture scandinavian = new Culture( "scandinavian" );
		static private Culture senegambian = new Culture( "senegambian" );
		static private Culture shawnee = new Culture( "shawnee" );
		static private Culture shona = new Culture( "shona" );
		static private Culture sikh = new Culture( "sikh" );
		static private Culture slavonic = new Culture( "slavonic" );
		static private Culture slovak = new Culture( "slovak" );
		static private Culture somali = new Culture( "somali" );
		static private Culture swahili = new Culture( "swahili" );
		static private Culture swiss = new Culture( "swiss" );
		static private Culture syrian = new Culture( "syrian" );
		static private Culture teremembe = new Culture( "teremembe" );
		static private Culture thai = new Culture( "thai" );
		static private Culture tibetan = new Culture( "tibetan" );
		static private Culture tuareg = new Culture( "tuareg" );
		static private Culture tupinamba = new Culture( "tupinamba" );
		static private Culture turkish = new Culture( "turkish" );
		static private Culture ugric = new Culture( "ugric" );
		static private Culture ukrainian = new Culture( "ukrainian" );
		static private Culture uzbehk = new Culture( "uzbehk" );
		static private Culture vietnamese = new Culture( "vietnamese" );
		static private Culture yorumba = new Culture( "yorumba" );
		static private Culture zapotek = new Culture( "zapotek" );
		#endregion

		static public Culture FromName( string name ) {
			switch ( name.ToLower() ) {
				case "none":				return None;
				case "abenaki":				return Abenaki;			
				case "aborigin":			return Aborigin;		
				case "afghani":				return Afghani;
				case "aka":					return Aka;
				case "albanian":			return Albanian;
				case "aleutian":			return Aleutian;
				case "amazonian":			return Amazonian;
				case "andean":				return Andean;
				case "arabic":				return Arabic;
				case "armenian":			return Armenian;
				case "ashanti":				return Ashanti;
				case "aztek":				return Aztek;
				case "altai":				return Altai;
				case "anglosaxon":			return Anglosaxon;
				case "baltic":				return Baltic;
				case "baluchi":				return Baluchi;
				case "bantu":				return Bantu;
				case "basque":				return Basque;
				case "bengali":				return Bengali;
				case "berber":				return Berber;
				case "burmanese":			return Burmanese;
				case "cambodian":			return Cambodian;
				case "canary":				return Canary;
				case "cantonese":			return Cantonese;
				case "caribbean":			return Caribbean;
				case "cherokee":			return Cherokee;
				case "cree":				return Cree;
				case "creek":				return Creek;
				case "czech":				return Czech;
				case "dravidian":			return Dravidian;
				case "dakota":				return Dakota;
				case "delaware":			return Delaware;
				case "dutch":				return Dutch;
				case "dyola":				return Dyola;
				case "ethiopian":			return Ethiopian;
				case "filippine":			return Filippine;
				case "french":				return French;
				case "gaelic":				return Gaelic;
				case "georgian":			return Georgian;
				case "german":				return German;
				case "greek":				return Greek;
				case "guajiro":				return Guajiro;
				case "gujarati":			return Gujarati;
				case "han":					return Han;
				case "hawaiian":			return Hawaiian;
				case "hindi":				return Hindi;
				case "huron":				return Huron;
				case "iberian":				return Iberian;
				case "indian":				return Indian;
				case "indonesian":			return Indonesian;
				case "inuit":				return Inuit;
				case "italian":				return Italian;
				case "iroquis":				return Iroquis;
				case "japanese":			return Japanese;
				case "javan":				return Javan;
				case "kongolese":			return Kongolese;
				case "khazak":				return Khazak;
				case "khmer":				return Khmer;
				case "korean":				return Korean;
				case "kurdish":				return Kurdish;
				case "laotian":				return Laotian;
				case "lithuanian":			return Lithuanian;
				case "madagasque":			return Madagasque;
				case "magyar":				return Magyar;
				case "malay":				return Malay;
				case "mali":				return Mali;
				case "maltese":				return Maltese;
				case "mayan":				return Mayan;
				case "melanese":			return Melanese;
				case "manchu":				return Manchu;
				case "marathi":				return Marathi;
				case "mataco":				return Mataco;
				case "mesoamerican":		return Mesoamerican;
				case "mississippian":		return Mississippian;
				case "mongol":				return Mongol;
				case "naskapi":				return Naskapi;
				case "navajo":				return Navajo;
				case "nubian":				return Nubian;
				case "patagonian":			return Patagonian;
				case "persian":				return Persian;
				case "polish":				return Polish;
				case "polynese":			return Polynese;
				case "russian":				return Russian;
				case "romanian":			return Romanian;
				case "ruthenian":			return Ruthenian;
				case "scandinavian":		return Scandinavian;
				case "senegambian":			return Senegambian;
				case "shawnee":				return Shawnee;
				case "shona":				return Shona;
				case "sikh":				return Sikh;
				case "slavonic":			return Slavonic;
				case "slovak":				return Slovak;
				case "somali":				return Somali;
				case "swahili":				return Swahili;
				case "swiss":				return Swiss;
				case "syrian":				return Syrian;
				case "teremembe":			return Teremembe;
				case "thai":				return Thai;
				case "tibetan":				return Tibetan;
				case "tuareg":				return Tuareg;
				case "tupinamba":			return Tupinamba;
				case "turkish":				return Turkish;
				case "ugric":				return Ugric;
				case "ukrainian":			return Ukrainian;
				case "uzbehk":				return Uzbehk;
				case "vietnamese":			return Vietnamese;
				case "yorumba":				return Yorumba;
				case "zapotek":				return Zapotek;
				default:					return None;
			}
		}
	}
}
