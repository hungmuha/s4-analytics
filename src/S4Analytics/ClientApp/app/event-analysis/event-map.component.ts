import { Component, ElementRef, Input, OnInit } from '@angular/core';
import * as ol from 'openlayers';
import { CrashService, CrashQuery } from './shared';
import { OptionsService } from '../shared';

@Component({
    selector: 'event-map',
    template: `<div id={{mapId}}></div>`
})
export class EventMapComponent implements OnInit {
    @Input() mapId: string;

    private olMap: ol.Map;
    private olView: ol.View;
    private olExtent: ol.Extent;

    constructor(
        private element: ElementRef,
        private crashService: CrashService,
        private optionService: OptionsService) { }

    ngOnInit() {

        let query: CrashQuery = {
            dateRange: { startDate: new Date('2017-04-01'), endDate: new Date('2017-04-07') }
        };

        this.optionService
            .getOptions()
            .subscribe(options => {
                let coordSys = options.coordinateSystems['WebMercator'];
                this.olExtent = [coordSys.mapExtent.minX, coordSys.mapExtent.minY, coordSys.mapExtent.maxX, coordSys.mapExtent.maxY];
                this.crashService.getCrashFeatures(query, this.olExtent).subscribe(eventResultSet => {
                    let clusterSource = new ol.source.Cluster({
                        distance: 100,
                        source: new ol.source.Vector({
                            features: (new ol.format.GeoJSON()).readFeatures(eventResultSet.featureCollection)
                        })
                    });

                    let styleCache = {};
                    let clusters = new ol.layer.Vector({
                        source: clusterSource,
                        style: function (feature) {
                            let size = feature.get('features').length as number;
                            if (eventResultSet.sampleMultiplier) {
                                size = Math.round(size * eventResultSet.sampleMultiplier);
                            }
                            let style = (styleCache as any)[size];
                            if (!style) {
                                style = new ol.style.Style({
                                    image: new ol.style.Circle({
                                        radius: 10,
                                        stroke: new ol.style.Stroke({
                                            color: '#fff'
                                        }),
                                        fill: new ol.style.Fill({
                                            color: '#3399CC'
                                        })
                                    }),
                                    text: new ol.style.Text({
                                        text: size.toString(),
                                        fill: new ol.style.Fill({
                                            color: '#fff'
                                        })
                                    })
                                });
                                (styleCache as any)[size] = style;
                            }
                            return style;
                        }
                    });

                    let raster = new ol.layer.Tile({
                        source: new ol.source.OSM()
                    });

                    this.olView = new ol.View({
                        center: [0, 0],
                        zoom: 2,
                        extent: this.olExtent
                    });

                    this.olMap = new ol.Map({
                        layers: [raster, clusters],
                        target: this.element.nativeElement.firstElementChild,
                        view: this.olView
                    });

                    // zoom to extent
                    this.olView.fit(this.olExtent);
                });
            });
    }
}
