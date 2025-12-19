namespace CitasMedicas.Core.Entities
{
    /// <summary>
    /// Esta clase define el identificador
    /// que sera heredado por las entidades .
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Identificador base que se puede reutilizar en varias entidades.
        /// en este caso en todas mis entidades
        /// </summary>
        /// <example>2</example>
        public int Id { get; set; }
    }
}
