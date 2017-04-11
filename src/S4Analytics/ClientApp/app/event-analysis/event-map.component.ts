import { Component, ElementRef, Input, OnInit } from '@angular/core';
import * as ol from 'openlayers';
import * as proj4x from 'proj4';
import { CrashService } from './shared';
import { OptionsService } from '../options.service';

// the typings haven't caught up with a recent change to proj4
// (see https://github.com/DefinitelyTyped/DefinitelyTyped/issues/15663)
let proj4 = (proj4x as any).default;

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
        let query: any = {
            dateRange: { startDate: '2017-04-01', endDate: '2017-04-07' }
        };

        // TODO: move projection details somewhere common / perform transforms in Oracle instead
        ol.proj.setProj4(proj4);
        proj4.defs('EPSG:3087', '+proj=aea +lat_1=24 +lat_2=31.5 +lat_0=24 +lon_0=-84 +x_0=400000 +y_0=0 +ellps=GRS80 +units=m +no_defs');
        let baseProj = ol.proj.get('EPSG:3857'); // web mercator
        let dataProj = ol.proj.get('EPSG:3087'); // fgdl albers

        this.optionService
            .getOptions()
            .subscribe(options => {
                this.olExtent = options.mapExtent as ol.Extent;
                let albersPoint1 = ol.proj.transform([this.olExtent[0], this.olExtent[1]], baseProj, dataProj);
                let albersPoint2 = ol.proj.transform([this.olExtent[2], this.olExtent[3]], baseProj, dataProj);
                let albersExtent: ol.Extent = [albersPoint1[0], albersPoint1[1], albersPoint2[0], albersPoint2[1]];
                this.crashService.getCrashPoints(query, albersExtent).subscribe(pointColl => {
                    let features = pointColl.points.map(point => new ol.Feature(new ol.geom.Point(ol.proj.transform([point.x, point.y], dataProj, baseProj))));

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
                    this.olView.fit(this.olExtent, this.olMap.getSize());
                });
            });
    }
}
