namespace MedicalSystem
{
    /// <summary>
    /// Модель пациента с 13 медицинскими параметрами для анализа
    /// </summary>
    internal class Patient
    {
        /// <summary>Возраст пациента</summary>
        public double Age { get; set; }

        /// <summary>Пол (0 или 1)</summary>
        public double Gender { get; set; }

        /// <summary>Тип боли в груди</summary>
        public double ChestPaintType { get; set; }

        /// <summary>Артериальное давление</summary>
        public double BloodPressure { get; set; }

        /// <summary>Уровень холестерола</summary>
        public double Cholestoral { get; set; }

        /// <summary>Уровень сахара в крови</summary>
        public double Sugar { get; set; }

        /// <summary>Электрокардиографический результат</summary>
        public double Electrocardiographic { get; set; }

        /// <summary>Максимальная частота пульса</summary>
        public double HeartRate { get; set; }

        /// <summary>Индуцированная стенокардия (0 или 1)</summary>
        public double InduceAngina { get; set; }

        /// <summary>Депрессия ST</summary>
        public double StDepression { get; set; }

        /// <summary>Наклон сегмента ST</summary>
        public double Slope { get; set; }

        /// <summary>Количество крупных сосудов</summary>
        public double MajorVesselsNumber { get; set; }

        /// <summary>Таллассемия</summary>
        public double Thal { get; set; }

        /// <summary>
        /// Возвращает все параметры пациента как массив для передачи в нейронную сеть
        /// </summary>
        internal double[] GetInfo()
        {
            return new double[]
            {
                Age,
                Gender,
                ChestPaintType,
                BloodPressure,
                Cholestoral,
                Sugar,
                Electrocardiographic,
                HeartRate,
                InduceAngina,
                StDepression,
                Slope,
                MajorVesselsNumber,
                Thal
            };
        }
    }
}
