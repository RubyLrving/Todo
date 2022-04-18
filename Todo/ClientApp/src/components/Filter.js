import React, { Component, useState } from 'react';
import { Home } from './Home';

export class Filter extends Component {
    static displayName = Filter.name;

    constructor(props) {
        super(props);
        this.state = { callback: this.props.callback };
        this.CallBackFunction = this.CallBackFunction.bind(this);
    }

    CallBackFunction(value) {
        this.state.callback(value);
    }

    render() {
        return (
            <div className="container">
                <a className="button" href="#" onClick={e => this.CallBackFunction("All")} >All</a>
                <a className="button" href="#" onClick={e => this.CallBackFunction("Todo")} >ToDo</a>
                <a className="button" href="#" onClick={e => this.CallBackFunction("Done")} >Done</a>
            </div>
        );
    }
}