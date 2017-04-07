import { Component, ElementRef, Input, OnInit } from '@angular/core';
import * as ol from 'openlayers';
import { CrashService } from './shared';

@Component({
    selector: 'event-map',
    template: `<div id={{mapId}}></div>`
})
export class EventMapComponent implements OnInit {
    @Input() mapId: string;

    private olMap: ol.Map;

    constructor(private element: ElementRef, private crashService: CrashService) { }

    ngOnInit() {
        let query: any = {
            dateRange: { "startDate": "2016-03-23", "endDate": "2017-03-22" }
        };
        this.crashService.getCrashPoints(query).subscribe(pointColl => {
            let features = pointColl.points.map(point => new ol.Feature(new ol.geom.Point(ol.proj.fromLonLat([point.x, point.y]))));

            let source = new ol.source.Vector({
                features: features
            });

            let clusterSource = new ol.source.Cluster({
                distance: 100,
                source: source
            });

            let styleCache = {};
            let clusters = new ol.layer.Vector({
                source: clusterSource,
                style: function (feature) {
                    let size = feature.get('features').length as number;
                    if (pointColl.sampleMultiplier) {
                        size = Math.round(size * pointColl.sampleMultiplier);
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

            this.olMap = new ol.Map({
                layers: [raster, clusters],
                target: this.element.nativeElement.firstElementChild,
                view: new ol.View({
                    center: [0, 0],
                    zoom: 2
                })
            });
        });
    }
}
