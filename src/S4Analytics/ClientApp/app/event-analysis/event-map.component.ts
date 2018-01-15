import { Component, ElementRef, Input, OnInit, HostListener } from '@angular/core';
import * as ol from 'openlayers';
import { CrashService, CrashQuery } from './shared';
import { AppStateService } from '../shared';

@Component({
    selector: 'event-map',
    template: `<div id="{{mapId}}"></div>`
})
export class EventMapComponent implements OnInit {
    @Input() mapId: string;

    private olMap: ol.Map;
    private olView: ol.View;
    private olExtent: ol.Extent;

    constructor(
        private element: ElementRef,
        private crashService: CrashService,
        private appState: AppStateService) { }

    ngOnInit() {

        //let query: CrashQuery = {
        //    dateRange: { startDate: new Date('2017-06-15'), endDate: new Date('2017-06-18') }
        //};

        let coordSys = this.appState.options.coordinateSystems['WebMercator'];
        this.olExtent = [coordSys.mapExtent.minX, coordSys.mapExtent.minY, coordSys.mapExtent.maxX, coordSys.mapExtent.maxY];
        let raster = new ol.layer.Tile({
            source: new ol.source.OSM()
        });

        this.olView = new ol.View({
            center: [0, 0],
            zoom: 2,
            extent: this.olExtent
        });

        this.olMap = new ol.Map({
            interactions: ol.interaction.defaults({ mouseWheelZoom: false }),
            layers: [raster],
            target: this.element.nativeElement.firstElementChild,
            view: this.olView
        });

        this.updateSize();

        // zoom to extent
        this.olView.fit(this.olExtent);
        /* this.crashService.getCrashFeatures(query, this.olExtent).subscribe(eventResultSet => {
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
                interactions: ol.interaction.defaults({ mouseWheelZoom: false }),
                layers: [raster, clusters],
                target: this.element.nativeElement.firstElementChild,
                view: this.olView
            });

            // zoom to extent
            this.olView.fit(this.olExtent);
        }); */
    }

    @HostListener('window:resize', [])
    updateSize() {
        // get references to elements
        var target = this.element.nativeElement.firstElementChild as HTMLElement; // target element (DIV)
        var component = target.parentElement as HTMLElement; // component element (EVENT-MAP)
        var container = component.parentElement as HTMLElement; // container element to conform to (probably DIV)
        // get container size
        var width = window.getComputedStyle(container).getPropertyValue('width');
        var height = window.getComputedStyle(container).getPropertyValue('height');
        var widthPx = Number(width.replace('px', ''));
        var heightPx = Number(height.replace('px', ''));
        // set size of target element
        target.style.width = width;
        target.style.height = height;
        // set size of openlayers map
        this.olMap.setSize([widthPx, heightPx]);
    }
}
