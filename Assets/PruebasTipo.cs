using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebasTipo : MonoBehaviour
{

    public abstract class Ser
    {
        public abstract string Morir();
    }

    public class Maligno : Ser
    {
        public override string Morir()
        {
            return "Mecagoentusputosmuertos";
        }
        public Maligno() { }
    }

    public class Benigno : Ser
    {
        public override string Morir()
        {
            return "Dale de comer al perro";
        }
        public Benigno() { }
    }

    public enum Seres
    {
        Benigno,
        Maligno
    }

    public Seres[] seresDefinidos;

    private Ser[] seres;

    // Use this for initialization
    void Start()
    {
        int length = seresDefinidos.Length;
        seres = new Ser[length];
        Ser ser;
        for (int i = 0; i < length; i++)
        {
            if (seresDefinidos[i] == Seres.Benigno)
            {
                ser = new Benigno();
            }
            else
            {
                ser = new Maligno();
            }
            seres[i] = ser;
        }
        ComprobarTipo<Maligno>();
        ComprobarTipo<Benigno>();
    }

    public void ComprobarTipo<T>() where T : Ser
    {
        foreach (Ser ser in seres)
        {
            ImprimirTipo<T>((T)((Ser)ser));
        }
    }

    public void ImprimirTipo<T>(Ser ser) where T : Ser
    {
        Debug.Log("Es benigno " + (ser is Benigno) + " Es maligno " + (ser is Maligno) + " Muere puto... " + ser.Morir());
    }


}
