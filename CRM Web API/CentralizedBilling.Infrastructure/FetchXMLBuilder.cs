using System;

namespace CentralizedBilling.Infrastructure
{
    public static class FetchXMLBuilder
    {
        public static string GetAccountXML()
        {
            return  $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='account'>
                                    <attribute name='name' />
                                    <attribute name='hisc_billingemail' />
                                    <attribute name='accountid' />
                                    <attribute name='address1_line1' />
                                    <attribute name='address1_line3' />
                                    <attribute name='address1_city' />
                                    <attribute name='address1_stateorprovince' />
                                    <attribute name='address1_postalcode' />
                                    <order attribute='name' descending='false' />
                                  </entity>
                                </fetch>";
        }
    }
}

