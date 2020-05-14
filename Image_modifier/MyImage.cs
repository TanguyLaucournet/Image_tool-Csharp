using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ProjetInfoS4
{
    class MyImage
    {
       public byte[] header = new byte[54];
        int typeBM;
        string typeImage;
        public int hauteur;
        public int largeur;
        public int tailleFichier;
        int nbbitscouleur;
       public Pixel[,] matricePixel;
        byte[] tabtemp = new byte[4];

        public MyImage(int largeur, int hauteur)
        {
            matricePixel = new Pixel[largeur, hauteur];
            this.hauteur = hauteur;
            this.largeur = largeur;
            typeImage = "BM";

        }
        public MyImage(MyImage img,int hauteur,int largeur)
        {
            this.typeImage = img.typeImage;
            this.header = img.header;
            typeImage = "BM";
            this.hauteur = hauteur;
            this.largeur = largeur;


        }
        public MyImage(string myfile)
        {
           
            
            byte[] file = File.ReadAllBytes(myfile);
            this.typeBM = file[0] + 1000 *file[1];
            
            Process.Start(myfile);

            byte [] temp= new byte [4];
            for (int k = 18; k < 22; k++) //Récupération largeur

                {
                temp[k - 18] = file[k];
              
                }

               this.largeur = Convertir_Endian_To_Int(temp);
          

            for (int k = 22; k < 26; k++) //Récupération hauteur
            {
                temp[k - 22] = file[k];
               
            }

            this.hauteur = Convertir_Endian_To_Int(temp);

            matricePixel = new Pixel[largeur, hauteur];
            Console.ReadKey();

           

            byte[] Header = new byte[53]; //Taille  du tableau pour le premier header 
            int debut = Header[11];//récuperation octect départ
           
           
           
            int j = 53;

            for ( int k = 28; k < 30; k++)
            {
                tabtemp[k - 28] = file[k];
            }
            tabtemp[2] = 0;
            tabtemp[3] = 0;
            this.nbbitscouleur = Convertir_Endian_To_Int(tabtemp);
         

            
                        for (int y = 0; y < hauteur; y++)
                        {
                            for (int x = 0; x < largeur; x++)
                            {

                                    temp = new byte[1];
                                    temp[0] = file[j - 1];
                                    int rouge = Convertir_Endian_To_Int(temp);

                                    temp[0] = file[j];
                                    int vert = Convertir_Endian_To_Int(temp);

                                    temp[0] = file[j+1];
                                    int bleu = Convertir_Endian_To_Int(temp);


                                Pixel p = new Pixel(rouge, vert, bleu);


                                matricePixel[x, y] = p;

                                j += 3;


                            }
                        }



             int i = 0;
            //Taille ficher
            temp = new byte[4];
            for (i = 2; i < 6; i++)
            {
                temp[i-2] = file[i];
            }
           this.tailleFichier = Convertir_Endian_To_Int(temp);
           
            //Nb bits couleur
            byte[] temp2 = new byte[3];
            for (i = 25; i < 28; i++)
            {
                temp2[i - 25] = file[i];
            }
            this.nbbitscouleur = Convertir_Endian_To_Int(temp);
           
            // Type image
            for (i = 0; i < 2; i++)
            {
                temp2[i] = file[i];
            }
            int TypeImageDec = Convertir_Endian_To_Int(temp);
            this.typeImage = char.ConvertFromUtf32(TypeImageDec);


            for (int y =0;y<54;y++)
            {
                header[y] = file[y];
            }
        }

        public void Save(string file)
        {
            
            byte[] myfile = new byte[54 + this.hauteur * this.largeur * 3];
            for (int i = 0; i < 54 + this.hauteur * this.largeur * 3; i++)
              { myfile[i] = Convert.ToByte(0);
                
            }
          
            for (int y = 0; y < 54; y++)
            {
                myfile[y] = header[y];
            }

            tabtemp = Convertir_Int_To_Endian(this.largeur);
            for (int i = 18; i < 22; i++)
            {
                myfile[i] = tabtemp[i - 18];
                
            }
            tabtemp = Convertir_Int_To_Endian(this.hauteur);
            for (int i = 22; i < 24; i++)
            {
                myfile[i] = tabtemp[i - 22];
              
            }
            tabtemp = Convertir_Int_To_Endian(this.tailleFichier);
            for (int i = 2; i < 6; i++)
            {
                myfile[i] = tabtemp[i - 2];
            }


            myfile[10] = 54;
            myfile[14] = 40;
            myfile[26] = 1;
            myfile[34] = 176;
            myfile[35] = 4;

            int k = 53;
            
            for (int y = 0; y < hauteur; y++)
            {
                for (int x = 0; x < largeur; x++)
                {
                   byte[] temp = new byte[1];

                    Pixel p = matricePixel[x, y];
                    int r = p.R;
                    temp = Convertir_Int_To_Endian(r);
                    myfile[k] = temp[0];
                    k ++;
                    
                    int g = p.G;
                    temp = Convertir_Int_To_Endian(g);
                    myfile[k] = temp[0];
                    k++;
                    int b = p.B;
                    temp = Convertir_Int_To_Endian(b);
                    myfile[k] = temp[0];
                    k++;
                }
            }
            
            File.WriteAllBytes(file, myfile);
            Process.Start(file);

        }



        public Pixel GetPixel(int x, int y)
        {
            return matricePixel[x, y];

        }

        public void SetPixel(int x, int y, int r, int g, int b)
        {
            Pixel p = new Pixel(r, g, b);
            matricePixel[x, y] = p;

        }
        public void SetPixel(int x, int y, Pixel p)
        {
            
            matricePixel[x, y] = p;

        }


        public int TailleFichier
        {
            get { return tailleFichier; }
        }
        public int Height
        {
            get { return hauteur; }
        }
        public int Width
        {
            get { return largeur; }
        }
        public string TypeImage
        {
            get { return typeImage; }
        }
        public int Nbbitscouleur
        {
            get { return nbbitscouleur; }
        }
        


        /// <summary>
        /// Conversion little Endian to INT
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
    
        public int Convertir_Endian_To_Int(byte[] tab)
        {
            int entier = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                entier += Convert.ToInt32(tab[i] * Math.Pow(256, i)); //faire sans Math.POw
               
            }
            return entier;
        }

        public byte[] Convertir_Int_To_Endian(int entier)
        {
            byte[] bites = new byte[4];
            int a = 0;
            int c = 0;
            int d = 0;
            while (entier >= 256)
            {
                a++;
                entier -= 256;
            }
            while (a >= 256)
            {
                c++;
                a -= 256;
            }
            while (c >= 256)
            { d++; c -= 256; }
            bites[0] = Convert.ToByte(entier);
            bites[1] = Convert.ToByte(a);
            bites[2] = Convert.ToByte(c);
            bites[3] = Convert.ToByte(d);
            return bites;
        }


     


        }   
}
