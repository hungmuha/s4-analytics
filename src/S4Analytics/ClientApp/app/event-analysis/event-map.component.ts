import { Component, ElementRef, Input, Output, EventEmitter, OnInit, HostListener } from '@angular/core';
import * as ol from 'openlayers';
import * as _ from 'lodash';
import { EventFeatureSet } from './shared';
import { AppStateService } from '../shared';

export enum BaseMapType {
    OpenStreetMap,
    ArcGisWorldImagery,
    ArcGisStreets
}

@Component({
    selector: 'event-map',
    template: `<div id="{{mapId}}">
                    <button-group [multipleSelect]="false"
                              [items]="['Cartographic', 'Aerial']"
                              [(ngModel)]="baseMapTypeSwitcher"
                              ></button-group>
               </div>`
})
export class EventMapComponent implements OnInit {
    @Input() mapId: string;
    @Input() set crashFeatureSet(value: EventFeatureSet) {
        this._crashFeatureSet = value;
        if (this.olMap !== undefined) {
            this.drawCrashFeatures();
        }
    }

    get crashFeatureSet(): EventFeatureSet {
        return this._crashFeatureSet;
    }
    @Input() set baseMapType(value: BaseMapType) {
        this._baseMapType = value;
        if (this.olMap !== undefined) {
            this.drawBaseMap();
        }
    }
    get baseMapType(): BaseMapType {
        return this._baseMapType;
    }
    @Output() extentChange = new EventEmitter<ol.Extent>();


    set baseMapTypeSwitcher(value: string) {
        switch (value) {
            case 'Cartographic':
                this._baseMapType = BaseMapType.ArcGisStreets;
                break;
            case 'Aerial':
                this._baseMapType = BaseMapType.ArcGisWorldImagery;
                break;
            default:
                this._baseMapType = BaseMapType.OpenStreetMap;
                break;
        }

        this.drawBaseMap();

    }

    private _baseMapType: BaseMapType = BaseMapType.OpenStreetMap;
    private _crashFeatureSet: EventFeatureSet;
    private olMap: ol.Map;
    private olView: ol.View;
    private olExtent: ol.Extent;
    private baseMapLayer: ol.layer.Tile;
    private crashClusterLayer: ol.layer.Vector;

    constructor(
        private element: ElementRef,
        private appState: AppStateService) { }

    ngOnInit() {
        this.createMap();
    }

    private createMap() {
        // set initial extent
        let coordSys = this.appState.options.coordinateSystems['WebMercator'];
        this.olExtent = [coordSys.mapExtent.minX, coordSys.mapExtent.minY, coordSys.mapExtent.maxX, coordSys.mapExtent.maxY];

        // create view
        this.olView = new ol.View({
            center: [0, 0],
            zoom: 2,
            extent: this.olExtent
        });

        // create map
        this.olMap = new ol.Map({
            target: this.element.nativeElement.firstElementChild,
            view: this.olView
        });

        // set up extent change notification
        let notifyExtentChanged = () => {
            let extent = this.olMap.getView().calculateExtent(this.olMap.getSize());
            this.extentChange.emit(extent);
        };
        this.olMap.on('moveend', _.debounce(notifyExtentChanged, 250));

        // update map size
        this.updateSize();

        // zoom to initial extent
        this.olView.fit(this.olExtent);

        // draw base map
        this.drawBaseMap();

        // draw features
        this.drawCrashFeatures();
    }

    private drawBaseMap() {
        // create layer if it doesn't exist
        if (this.baseMapLayer === undefined) {
            this.baseMapLayer = new ol.layer.Tile();
            this.olMap.addLayer(this.baseMapLayer);
        }

        let source: ol.source.Source;

        // set layer source
        switch (this.baseMapType) {
            case BaseMapType.OpenStreetMap:
                source = new ol.source.OSM();
                break;
            case BaseMapType.ArcGisStreets:
                source = new ol.source.TileArcGISRest({
                    url: 'http://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer'
                });
                break;
            case BaseMapType.ArcGisWorldImagery:
                source = new ol.source.TileArcGISRest({
                    url: 'http://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer'
                });
                break;
            default:
                source = new ol.source.OSM();
                break;
        }
        this.baseMapLayer.setSource(source);


    }

    private drawCrashFeatures() {
        // create layer if it doesn't exist
        if (this.crashClusterLayer === undefined) {
            this.crashClusterLayer = new ol.layer.Vector();
            this.olMap.addLayer(this.crashClusterLayer);
        }

        // set layer source
        let clusterSource = new ol.source.Cluster({
            distance: 100,
            source: new ol.source.Vector({
                features: this.crashFeatureSet !== undefined
                    ? (new ol.format.GeoJSON()).readFeatures(this.crashFeatureSet.featureCollection)
                    : []
            })
        });
        this.crashClusterLayer.setSource(clusterSource);

        // set cluster styles
        let styleCache = {};
        let clusterStyle = (feature: any) => {
            let size = feature.get('features').length as number;
            if (this.crashFeatureSet && this.crashFeatureSet.sampleMultiplier) {
                size = Math.round(size * this.crashFeatureSet.sampleMultiplier);
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
        };
        this.crashClusterLayer.setStyle(clusterStyle);
    }

    @HostListener('window:resize', [])
    private updateSize() {
        // get references to elements
        let target = this.element.nativeElement.firstElementChild as HTMLElement; // target element (DIV)
        let component = target.parentElement as HTMLElement; // component element (EVENT-MAP)
        let container = component.parentElement as HTMLElement; // container element to conform to (probably DIV)
        // get container size
        let width = window.getComputedStyle(container).getPropertyValue('width');
        let height = window.getComputedStyle(container).getPropertyValue('height');
        let widthPx = Number(width.replace('px', ''));
        let heightPx = Number(height.replace('px', ''));
        // set size of target element
        target.style.width = width;
        target.style.height = height;
        // set size of openlayers map
        this.olMap.setSize([widthPx, heightPx]);
    }
}
