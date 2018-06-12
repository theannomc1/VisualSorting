﻿using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using VisualSorting.SortingAlgorithms;

namespace VisualSorting.UserInterface
{
    public class UserInterfaceViewModel : ViewModelBase
    {
        #region Eigenschaften / Variablen

        public Thread ThisThread;

        public SeriesCollection SeriesCollection { get; set; }

        private IEnumerable<int> numbers;
        public IEnumerable<int> Numbers
        {
            get { return numbers; }
            set { if (numbers != value) numbers = value; OnPropertyChanged(); }
        }

        public List<SortingBase> PossibleSortingAlgorithm { get; set; }

        public SortingBase SelectedSorting { get; set; }

        private bool working;

        public bool Working
        {
            get { return !working; }
            set
            {
                if (value != working)
                    working = value;
                Dispatcher.FromThread(ThisThread).Invoke(() =>
                {
                    MainWindow.Border = (working) ? MainWindow.ORANGE_BRUSH : MainWindow.GREEN_BRUSH;
                    MainWindow.StatusbarText = (working) ? MainWindow.STATUS_WORKING : MainWindow.STATUS_DONE;
                });
                OnPropertyChanged();
            }
        }

        private int delayValue = 100;

        public int DelayValue
        {
            get { return delayValue; }
            set { delayValue = value; Delay = value; }
        }

        public int AmountValue { get; set; } = 30;
        public static int Delay { get; set; } = 100;

        public ICommand Sort { get; set; }
        public ICommand Randomize { get; set; }
        public ICommand RandomizeAsc { get; set; }
        public ICommand RandomizeDesc { get; set; }

        private readonly Random random = new Random();

        public Func<int, int> FuncRandomNumbers;
        public Func<int, int> FuncAscNumbers;
        public Func<int, int> FuncDescNumbers;

        #endregion Eigenschaften / Variablen

        public UserInterfaceViewModel()
        {
            ThisThread = Dispatcher.CurrentDispatcher.Thread;

            FuncRandomNumbers = _ => random.Next(1, 51);
            FuncAscNumbers = x => x;
            FuncDescNumbers = x => AmountValue - x + 1;

            PossibleSortingAlgorithm = new List<SortingBase>()
            {
                new BubbleSort(null),
                new SelectionSort(null),
                new InsertionSort(null)
            };

            SeriesCollection = new SeriesCollection()
            {
                new ColumnSeries()
            };
            GenerateNumbers(FuncRandomNumbers);

            Sort = CreateCommand(_ => StartSorting());
            Randomize = CreateCommand(_ => GenerateNumbers(FuncRandomNumbers));
            RandomizeAsc = CreateCommand(_ => GenerateNumbers(FuncAscNumbers));
            RandomizeDesc = CreateCommand(_ => GenerateNumbers(FuncDescNumbers));
        }

        private void StartSorting()
        {
            SortingBase sorter = null;

            if (SelectedSorting is BubbleSort)
                sorter = new BubbleSort(Numbers);
            else if (SelectedSorting is SelectionSort)
                sorter = new SelectionSort(Numbers);
            else if (SelectedSorting is InsertionSort)
                sorter = new InsertionSort(Numbers);
            // ...

            if (sorter != null)
                SortBy(sorter);
        }

        private async void SortBy(SortingBase selectedSorting)
        {
            selectedSorting.NumbersUpdated += (s, e) => UpdateGraph(selectedSorting.NumberArray);

            Working = true;
            await selectedSorting.Sort().ConfigureAwait(false);
            Working = false;
        }

        private void UpdateGraph(IEnumerable<int> numbers)
        {
            Dispatcher.FromThread(ThisThread).Invoke(() =>
            {
                ChartValues<int> vs = new ChartValues<int>();
                vs.AddRange(numbers);
                SeriesCollection[0].Values = vs;
                this.Numbers = numbers;
            });
        }

        private void GenerateNumbers(Func<int, int> func)
        {
            List<int> nums = new List<int>();
            for (int i = 1; i <= AmountValue; i++)
            {
                nums.Add(func(i));
            }
            UpdateGraph(nums);
        }
    }
}