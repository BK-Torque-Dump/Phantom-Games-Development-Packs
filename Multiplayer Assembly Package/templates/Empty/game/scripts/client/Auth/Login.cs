//Login.cs
//Robert Fritzen (Phantom139)
//(c)Phantom Games Development 2011
//This script handles the log-in process

function getAccountList() {
   %account = 0;
   if(!isFile(GetUserDataPath() @ "accounts/certs.xml")) {
      error("AUTH 001: Missing certs.xml file");
      return;
   }
   %xml = new SimXMLDocument() {};
   %xml.loadFile(GetUserDataPath() @ "accounts/certs.xml");
   %xml.pushChildElement("AccountList");
   %xml.pushFirstChildElement("AccountData");
   while(true) {
      %xml.pushFirstChildElement("Account");
      %username = %xml.getData();
      $Auth::AccountName[%account] = %username;
      %account++;
      
      %xml.popElement();
      if (!%xml.nextSiblingElement("AccountData")) break;
   }

   %xml.clear();
   %xml.delete();
}

function getPublicCertificate(%name) {
   %xml = new SimXMLDocument() {};
   %xml.loadFile(GetUserDataPath() @ "accounts/certs.xml");
   %xml.pushChildElement("AccountList");
   %xml.pushFirstChildElement("AccountData");
   while(true) {
      %xml.pushFirstChildElement("Account");
      %username = %xml.getData();
      if(strlwr(%username) $= strlwr(%name)) {
         %xml.nextSiblingElement("AccountGUID");
         %guid = %xml.getData();
         %xml.nextSiblingElement("AccountEMAIL");
         %email = %xml.getData();
         %xml.nextSiblingElement("AccountRSA");
         %ensig = %xml.getData();

         %e = getSubStr(%ensig, 0, strPos(%ensig, ":"));
         %ensig = getSubStr(%ensig, strPos(%ensig, ":")+1, strlen(%ensig));
         %n = getSubStr(%ensig, 0, strPos(%ensig, ":"));
         %sig = getSubStr(%ensig, strPos(%ensig, ":")+1, strlen(%ensig));

         %line = %username TAB %guid TAB %email TAB %e TAB %n TAB %sig;
         %xml.clear();
         %xml.delete();
         return %line;
      }
      else {
         %xml.popElement();
         if (!%xml.nextSiblingElement("AccountData")) break;
      }
   }
   %xml.clear();
   %xml.delete();
   return "NOT_FOUND";
}

function getPrivateCertificate(%name) {
   %xml = new SimXMLDocument() {};
   %xml.loadFile(GetUserDataPath() @ "accounts/certs.xml");
   %xml.pushChildElement("AccountList");
   %xml.pushFirstChildElement("AccountData");
   while(true) {
      %xml.pushFirstChildElement("Account");
      %username = %xml.getData();
      if(strlwr(%username) $= strlwr(%name)) {
         %xml.nextSiblingElement("AccountRSAP");
         %rsaD = %xml.getData();
         %line = %username TAB %rsaD;

         %xml.clear();
         %xml.delete();
         return %line;
      }
      else {
         %xml.popElement();
         if (!%xml.nextSiblingElement("AccountData")) break;
      }
   }
   %xml.clear();
   %xml.delete();
   return "NOT_FOUND";
}