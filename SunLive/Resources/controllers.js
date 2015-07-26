rndApp.controller('dashboardController', function ($scope, $location, $filter, chartService) {
    

    $scope.paintTotalMessages = function (fromdate, toDate) {
        chartService.getTotalMessages(fromdate, toDate).then(function (data) {
            $scope.totalMessages = data;
        });
    };

    $scope.paintPerDayInfo = function (selectedDay) {
        $scope.dayInfoConfig = {
            options: {
                chart: {
                    type: 'column',
                    height: 375,
                },

                legend: {
                    enabled: false
                },
                exporting: {
                    enabled: false
                },
                //tooltip: {
                //    formatter: function () {
                //        var tooltipValue = '';
                //        if (this.point.x == 0) {
                //            tooltipValue += 'Facility Avg. items/case: <b>' + this.y + '</b>';
                //        }
                //        else if (this.point.x == 1) {
                //            tooltipValue += 'Cohort Avg. items/case: <b>' + this.y + '</b>';
                //        }
                //        return tooltipValue;
                //    }
                //},
                plotOptions: {
                    series: {
                        dataLabels: {
                            enabled: true,
                            formatter: function () {

                            },
                            useHTML: true
                        },
                    }
                },
            },
            series: [{
                color: '#7cb5ec',
                name: "Messages",
                data: selectedDay.messages
            }],
            title:
            {
                text: 'Messages Per TimeSlot'
            },
            xAxis: {
                categories: $scope.categories,
                title: {
                    text: 'Time slot (in half hours)'
                }
            },
            yAxis: {
                title: {
                    text: 'Messages'
                },
                min: 0,
            },
            loading: false,
            credits: {
                enabled: false
            },

        };
    };
    $scope.dayClickOnChart = function (point)
    {

        var found = $filter('filter')($scope.perDayInfo, { dateTime: point.x}, true);
        if (found.length > 0)
        {
            $scope.selectedDay = found[0];
            $scope.paintPerDayInfo($scope.selectedDay);
        }

        $scope.$apply();
    }
    
    
 
    chartService.getPerDayInfo().then(function (data) {
        $scope.categories = data.categories;
        $scope.perDayInfo = data.perDayInfo;

        var firstAvailableDay = data.perDayInfo[0];
        var lastAvailableDay = data.perDayInfo[data.perDayInfo.length - 1];
        $scope.selectedDay = lastAvailableDay;

        $scope.paintPerDayInfo(lastAvailableDay);

        $scope.paintTotalMessages(firstAvailableDay.dateTime, lastAvailableDay.dateTime);

    });
    

    chartService.getUniqueChartData().then(function (incomingData) {
        console.log(incomingData);
        //$scope.chartConfig1.series = incomingData;

        $scope.uniqueDataChart = {
            options: {
                chart: {
                    type: 'line',
                    zoomType: 'x',
                    //events: {
                    //    selection: function (event) {
                    //        if (event.xAxis) {
                    //            console.log(event.xAxis[0].min + " -----" + event.xAxis[0].max);
                    //        } else {
                    //            console.log('Selection reset');
                    //        }
                    //    }
                    //}

                },

                colors: ['#7cb5ec', '#2b908f'],

                plotOptions: {
                    series: {
                        cursor: 'pointer',
                        point: {
                            events: {
                                click: function () {
                                    $scope.dayClickOnChart(this);
                                },
                            }
                        }
                    }
                },
                navigator: { enabled: true }
            },
            colors: ['#7cb5ec', '#2b908f'],

            series: incomingData,
            title: {
                text: 'Time Series: Messages & Users'
            },
            xAxis: {
                type: 'datetime',
                events: {
                    setExtremes: function(e)
                    {
                        $scope.paintTotalMessages(e.min, e.max);
                    }
                }
            },
            loading: false,
            useHighStocks: true,
            loading: false,
            credits: {
                enabled: false
            },

        };
    });
        
});
