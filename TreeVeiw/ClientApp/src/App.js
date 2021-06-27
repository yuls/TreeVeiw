import React, { Component } from 'react';

import { TreeView } from './components/TreeView';

export default class App extends Component {
  static displayName = App.name;

  render () {
      return (<TreeView />);
  }
}
