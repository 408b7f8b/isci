using System;
using System.Collections.Generic;
using isci;
using isci.Anwendungen;
using isci.Daten;
using isci.Konfiguration;
using isci.Allgemein;

namespace modellgen
{
    class Program
    {
        static void Main(string[] args)
        {
            var anwSystemabbilder = new Basis()
            {
                Identifikation = "anwendung.Systemabbilder",
                Name = "Systemabbilder",
                Beschreibung = "Anwendung mit Systemabbildung für alle Einzelsysteme",
                Datenmodelle = new List<Datenmodell>(),
                Ereignisse = new List<Ereignis>(),
                Funktionen = new List<Funktion>(),
                ReferenzierteModelle = new List<string>(),
                Konfigurationselemente = new Dictionary<string, List<Konfigurationselement>>(),
                Konfigurationspakete = new Dictionary<string, Dictionary<string, string>>(){
                    {"*", new Dictionary<string, string>(){{"abbild", "isci.abbild"}}}
                }
            };

            var anwZykluszeit = new Basis()
            {
                Identifikation = "anwendung.Zykluszeit",
                Name = "Zykluszeit",
                Beschreibung = "Anwendung zur Zykluszeitermittlung für alle Einzelsysteme",
                Datenmodelle = new List<Datenmodell>(),
                Ereignisse = new List<Ereignis>(),
                Funktionen = new List<Funktion>(),
                ReferenzierteModelle = new List<string>(),
                Konfigurationselemente = new Dictionary<string, List<Konfigurationselement>>(),
                Konfigurationspakete = new Dictionary<string, Dictionary<string, string>>(){
                    {"*", new Dictionary<string, string>(){{"zykluszeit", "isci.zykluszeit"}}}
                }
            };

            var anwIntegrationStandard = new Basis()
            {
                Identifikation = "anwendung.IntegrationStandard",
                Name = "IntegrationStandard",
                Beschreibung = "Anwendung zur Standardintegration für alle Einzelsysteme",
                Datenmodelle = new List<Datenmodell>(),
                Ereignisse = new List<Ereignis>(),
                Funktionen = new List<Funktion>(),
                ReferenzierteModelle = new List<string>(),
                Konfigurationselemente = new Dictionary<string, List<Konfigurationselement>>(),
                Konfigurationspakete = new Dictionary<string, Dictionary<string, string>>() {
                    {"*", new Dictionary<string, string>(){{"integration", "isci.integration"}, {"link", "isci.link"}}}
                }
            };

            var anwValidierung = new Instanz()
            {
                Identifikation = "anwendung.Validierung",
                Name = "Validierungsanwendung",
                Beschreibung = "Validierungsanwendung",
                Datenmodelle = new List<Datenmodell>(),
                Ereignisse = new List<Ereignis>(),
                Funktionen = new List<Funktion>(),
                ReferenzierteModelle = new List<string>()
                {
                    "anwendung.Zykluszeit",
                    "anwendung.IntegrationStandard",
                    "anwendung.Systemabbilder"
                },
                Konfigurationselemente = new Dictionary<string, List<Konfigurationselement>>(),
                Konfigurationspakete = new Dictionary<string, Dictionary<string, string>>() {
                    {"RevPiA", new Dictionary<string, string>(){{"verarbeitung", "isci.validverarbeitung"}}},
                    {"RevPiB", new Dictionary<string, string>(){{"revpiea", "isci.revpiea"}}}
                },
                Ressourcenkartierung = new Dictionary<string, Dictionary<string, string>>()
                {
                    {
                        "anwendung.IntegrationStandard", new Dictionary<string, string>(){{"*", "*"}}
                    },
                    {
                        "anwendung.Zykluszeit", new Dictionary<string, string>(){{"*", "*"}}
                    },
                    {
                        "anwendung.Systemabbilder", new Dictionary<string, string>(){{"*", "*"}}
                    }
                },
                Ausführungstransitionen = new Dictionary<string, Dictionary<string, List<Ausführungstransition>>>()
                {
                    {
                        "RevPiA", new Dictionary<string, List<Ausführungstransition>>() {
                            {
                                "zykluszeit", new List<Ausführungstransition>() {
                                    new Ausführungstransition(0,1)
                                }
                            },
                            {
                                "integration", new List<Ausführungstransition>() {
                                    new Ausführungstransition(1,2),
                                    new Ausführungstransition(3,0)
                                }
                            },
                            {
                                "verarbeitung", new List<Ausführungstransition>() {
                                    new Ausführungstransition(2,3)
                                }
                            }
                        }
                    },
                    {
                        "RevPiB", new Dictionary<string, List<Ausführungstransition>>() {
                            {
                                "zykluszeit", new List<Ausführungstransition>() {
                                    new Ausführungstransition(0,1)
                                }
                            },
                            {
                                "integration", new List<Ausführungstransition>() {
                                    new Ausführungstransition(1,2),
                                    new Ausführungstransition(3,0)
                                }
                            },
                            {
                                "revpiea", new List<Ausführungstransition>() {
                                    new Ausführungstransition(2,3)
                                }
                            }
                        }
                    }
                }
            };

            var anwBasismaschine = new Basis()
            {
                Identifikation = "anwendung.CncStandard",
                Name = "Basismaschine CNC",
                Beschreibung = "Standardanwendung",
                Datenmodelle = new List<Datenmodell>()
                {
                    new Datenmodell("CncStandard"){
                        Dateneinträge = new ListeDateneintraege()
                        {
                            /*new Dateneintrag()
                            {
                                Identifikation = "Start",
                                type = Datentypen.Bool,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "Halt",
                                type = Datentypen.Bool,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "nIst",
                                type = Datentypen.Int32,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "nSoll",
                                type = Datentypen.Int32,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "pXist",
                                type = Datentypen.Float,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "pXsoll",
                                type = Datentypen.Float,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "vXist",
                                type = Datentypen.Float,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "vXsoll",
                                type = Datentypen.Float,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "pYist",
                                type = Datentypen.Float,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "pYsoll",
                                type = Datentypen.Float,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "vYist",
                                type = Datentypen.Float,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "vYsoll",
                                type = Datentypen.Float,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "pZist",
                                type = Datentypen.Float,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "pZsoll",
                                type = Datentypen.Float,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "vZist",
                                type = Datentypen.Float,
                                value = false
                            },
                            new Dateneintrag()
                            {
                                Identifikation = "vZsoll",
                                type = Datentypen.Float,
                                value = false
                            }*/
                        },
                        Links = new VerweislisteDateneintraege()
                        {
                            {"ns=AchseX;s=pXist", new List<string>(){"ns=CNC;s=pXist"}},
                            {"ns=CNC;s=pXsoll", new List<string>(){"ns=AchseX;s=pXsoll"}},
                            {"ns=AchseX;s=vXist", new List<string>(){"ns=CNC;s=vXist"}},
                            {"ns=CNC;s=vXsoll", new List<string>(){"ns=AchseX;s=vXsoll"}},

                            {"ns=AchseY;s=pYist", new List<string>(){"ns=CNC;s=pYist"}},
                            {"ns=CNC;s=pYsoll", new List<string>(){"ns=AchseY;s=pYsoll"}},
                            {"ns=AchseY;s=vYist", new List<string>(){"ns=CNC;s=vYist"}},
                            {"ns=CNC;s=vYsoll", new List<string>(){"ns=AchseY;s=vYsoll"}},

                            {"ns=AchseZ;s=pZist", new List<string>(){"ns=CNC;s=pZist"}},
                            {"ns=CNC;s=pZsoll", new List<string>(){"ns=AchseZ;s=pZsoll"}},
                            {"ns=AchseZ;s=vZist", new List<string>(){"ns=CNC;s=vZist"}},
                            {"ns=CNC;s=vZsoll", new List<string>(){"ns=AchseZ;s=vZsoll"}},

                            {"ns=Spindel;s=nIst", new List<string>(){"ns=CNC;s=nSoll"}},
                            {"ns=CNC;s=nSoll", new List<string>(){"ns=Spindel;s=nIst"}}
                        }
                    },
                },
                Ereignisse = new List<Ereignis>()
                {
                    new Ereignis()
                    {
                        Identifikation = "eGestartet",
                        Beschreibung = "Maschine gestartet",
                        Name = "Gestartet",
                        Ausloeser = "ns=CNC;s=Gestartet",
                        Elemente = new List<string>(){"ns=CNC;s=Gestartet"}
                    },
                    new Ereignis()
                    {
                        Identifikation = "eGehalten",
                        Beschreibung = "Maschine gehalten",
                        Name = "Gehalten",
                        Ausloeser = "ns=CNC;s=Gehalten",
                        Elemente = new List<string>(){"ns=CNC;s=Gehalten"}
                    },
                    new Ereignis()
                    {
                        Identifikation = "eAbschluss",
                        Beschreibung = "Bearbeitung abgeschlossen",
                        Name = "Abschluss",
                        Ausloeser = "ns=CNC;s=Abschluss",
                        Elemente = new List<string>(){"ns=CNC;s=Abschluss"}
                    },
                    new Ereignis()
                    {
                        Identifikation = "eStatus",
                        Beschreibung = "Status verteilen",
                        Name = "Status",
                        Ausloeser = "ns=CNC;s=Status",
                        Elemente = new List<string>(){
                            "ns=CNC;s=AktuellerSchritt",
                            "ns=CNC;s=pXist",
                            "ns=CNC;s=pYist",
                            "ns=CNC;s=pZist",
                            "ns=CNC;s=nIst"
                        }
                    }
                },
                Funktionen = new List<Funktion>()
                {
                    new Funktion()
                    {
                        Identifikation = "fStart",
                        Beschreibung = "Maschine Start",
                        Name = "Start",
                        Ziele = new List<string>(){
                            "ns=CNC;s=Start"
                        }
                    },
                    new Funktion()
                    {
                        Identifikation = "fHalt",
                        Beschreibung = "Maschine Halt",
                        Name = "Halt",
                        Ziele = new List<string>(){
                            "ns=CNC;s=Halt"
                        }
                    },
                    new Funktion()
                    {
                        Identifikation = "fSchritte",
                        Beschreibung = "Maschine Schritte",
                        Name = "Schritte",
                        Ziele = new List<string>(){
                            "ns=CNC;s=Schritte"
                        }
                    }
                },
                ReferenzierteModelle = new List<string>()
                {
                    "anwendung.Zykluszeit",
                    "anwendung.IntegrationStandard"
                },
                Konfigurationselemente = new Dictionary<string, List<Konfigurationselement>>()
                {
                    {
                        "SteuerungX", new List<Konfigurationselement>(){
                        {
                            new Konfigurationselement(){typ = "Parameter", vorgang = new isci.Konfiguration.Parameter() {
                                Ordner = "AchseX",
                                Variablen = new Dictionary<string, string>(){{"Steigung", "5"}, {"Aufloesung", "400"}}
                            }}
                        }
                    }},
                    {
                        "SteuerungY", new List<Konfigurationselement>(){
                        {
                            new Konfigurationselement(){typ = "Parameter", vorgang = new isci.Konfiguration.Parameter() {
                                Ordner = "AchseY",
                                Variablen = new Dictionary<string, string>(){{"Steigung", "5"}, {"Aufloesung", "400"}}
                            }}
                        }
                    }},
                    {
                        "SteuerungZ", new List<Konfigurationselement>(){
                        {
                            new Konfigurationselement(){typ = "Parameter", vorgang = new isci.Konfiguration.Parameter() {
                                Ordner = "AchseZ",
                                Variablen = new Dictionary<string, string>(){{"Steigung", "8"}, {"Aufloesung", "400"}}
                            }}
                        }
                    }},
                    {
                        "SteuerungSpind", new List<Konfigurationselement>(){
                        {
                            new Konfigurationselement(){typ = "Parameter", vorgang = new isci.Konfiguration.Parameter() {
                                Ordner = "Spindel",
                                Variablen = new Dictionary<string, string>(){{"Adresse", "1"}}
                            }}
                        }
                    }}
                },
                Konfigurationspakete = new Dictionary<string, Dictionary<string, string>>(){
                    {"SteuerungX", new Dictionary<string, string>(){{"AchseX", "isci.schrittmotorachse"}}},
                    {"SteuerungY", new Dictionary<string, string>(){{"AchseY", "isci.schrittmotorachse"}}},
                    {"SteuerungZ", new Dictionary<string, string>(){{"AchseZ", "isci.schrittmotorachse"}}},
                    {"SteuerungSpind", new Dictionary<string, string>(){{"Spindel", "isci.modbusspindel"}}},
                    {"SteuerungCnc", new Dictionary<string, string>(){{"CNC", "isci.dreiachsansteuerung"}}},
                    {"SchnittstelleMsb", new Dictionary<string, string>(){{"MSB", "isci.msb"}}}
                }
            };

            var BasismaschineInstanz = new Instanz()
            {
                Identifikation = "anwendung.Maschine1",
                Name = "CNC-Maschine 1",
                Beschreibung = "Instanz",
                Datenmodelle = new List<Datenmodell>(),
                Ereignisse = new List<Ereignis>(),
                Funktionen = new List<Funktion>(),
                ReferenzierteModelle = new List<string>(){
                    "anwendung.Systemabbilder",
                    "anwendung.CncStandard"
                },
                Konfigurationselemente = new Dictionary<string, List<Konfigurationselement>>() {
                    {
                        "RevPiXYZ", new List<Konfigurationselement>(){
                            new Konfigurationselement(){typ = "Parameter", vorgang = new isci.Konfiguration.Parameter() {
                                Ordner = "AchseX",
                                Variablen = new Dictionary<string, string>(){{"Id", "101"}}
                            }},
                            new Konfigurationselement(){typ = "Parameter", vorgang = new isci.Konfiguration.Parameter() {
                                Ordner = "AchseY",
                                Variablen = new Dictionary<string, string>(){{"Id", "102"}}
                            }},
                            new Konfigurationselement(){typ = "Parameter", vorgang = new isci.Konfiguration.Parameter() {
                                Ordner = "AchseZ",
                                Variablen = new Dictionary<string, string>(){{"Id", "103"}}
                            }}
                        }
                    }
                },
                Konfigurationspakete = new Dictionary<string, Dictionary<string, string>>(),
                Ressourcenkartierung = new Dictionary<string, Dictionary<string, string>>()
                {
                    {
                        "anwendung.IntegrationStandard", new Dictionary<string, string>(){{"*", "*"}}
                    },
                    {
                        "anwendung.Zykluszeit", new Dictionary<string, string>(){{"*", "*"}}
                    },
                    {
                        "anwendung.Systemabbilder", new Dictionary<string, string>(){{"*", "*"}}
                    },
                    {
                        "anwendung.CncStandard", new Dictionary<string, string>(){
                            {"SteuerungX", "RevPiXYZ"},
                            {"SteuerungY", "RevPiXYZ"},
                            {"SteuerungZ", "RevPiXYZ"},
                            {"SteuerungSpind", "RevPiSpind"},
                            {"SteuerungCnc", "PC"},
                            {"SchnittstelleMsb", "PC"}
                        }
                    }
                },
                Ausführungstransitionen = new Dictionary<string, Dictionary<string, List<Ausführungstransition>>>()
                {
                    {
                        "RevPiXYZ", new Dictionary<string, List<Ausführungstransition>>() {
                            {
                                "zykluszeit", new List<Ausführungstransition>() {
                                    new Ausführungstransition(0,1)
                                }
                            },
                            {
                                "integration", new List<Ausführungstransition>() {
                                    new Ausführungstransition(1,2),
                                    new Ausführungstransition(7,0)
                                }
                            },
                            {
                                "AchseX", new List<Ausführungstransition>() {
                                    new Ausführungstransition(3,4)
                                }
                            },
                            {
                                "AchseY", new List<Ausführungstransition>() {
                                    new Ausführungstransition(4,5)
                                }
                            },
                            {
                                "AchseZ", new List<Ausführungstransition>() {
                                    new Ausführungstransition(5,6)
                                }
                            },
                            {
                                "link", new List<Ausführungstransition>() {
                                    new Ausführungstransition(2,3),
                                    new Ausführungstransition(6,7)
                                }
                            }
                        }
                    },
                    {
                        "RevPiSpind", new Dictionary<string, List<Ausführungstransition>>() {
                            {
                                "zykluszeit", new List<Ausführungstransition>() {
                                    new Ausführungstransition(0,1)
                                }
                            },
                            {
                                "integration", new List<Ausführungstransition>() {
                                    new Ausführungstransition(1,2),
                                    new Ausführungstransition(5,0)
                                }
                            },
                            {
                                "Spindel", new List<Ausführungstransition>() {
                                    new Ausführungstransition(3,4)
                                }
                            },
                            {
                                "link", new List<Ausführungstransition>() {
                                    new Ausführungstransition(2,3),
                                    new Ausführungstransition(4,5)
                                }
                            }
                        }
                    },
                    {
                        "PC", new Dictionary<string, List<Ausführungstransition>>() {
                            {
                                "zykluszeit", new List<Ausführungstransition>() {
                                    new Ausführungstransition(0,1)
                                }
                            },
                            {
                                "integration", new List<Ausführungstransition>() {
                                    new Ausführungstransition(1,2),
                                    new Ausführungstransition(6,0)
                                }
                            },
                            {
                                "MSB", new List<Ausführungstransition>() {
                                    new Ausführungstransition(2,3)
                                }
                            },
                            {
                                "CNC", new List<Ausführungstransition>() {
                                    new Ausführungstransition(4,5)
                                }
                            },
                            {
                                "link", new List<Ausführungstransition>() {
                                    new Ausführungstransition(3,4),
                                    new Ausführungstransition(5,6)
                                }
                            }
                        }
                    }
                }
            };

            anwValidierung.Speichern();
            anwIntegrationStandard.Speichern();
            anwZykluszeit.Speichern();
            anwSystemabbilder.Speichern();
            anwBasismaschine.Speichern();
            BasismaschineInstanz.Speichern();
        }
    }
}
