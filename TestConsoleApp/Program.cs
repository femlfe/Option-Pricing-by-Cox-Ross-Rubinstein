using CRR_Model.Classes;

Option option = new Option();

option.S = 100;
option.K = 110;
option.Expiry = DateTime.Now.AddDays(90);
option.Sigma = 0.2;
option.R = 0.05;


Console.WriteLine(CRR.Delta(option,true,true));

Console.WriteLine(CRR.Delta(option, false, true));