import React, {  Component } from 'react';
import './TreeView.css';
import loading from '../images/loading.gif';

const NodeTypes = Object.freeze({ Folder: 0, File: 1 });

const OrderBy = Object.freeze({ Name: 0, Size: 1 });

export class TreeView extends Component {

    orderNodesBy = OrderBy.Name;

    constructor(props) {
        super(props);
        this.state = {
            treeElements: [],
            loading: true,
            dragNode: null
        };
    }

    componentDidMount() {
        this.populateTreeViewData();
    }

    getRootNodes = () => {
        const  nodes = this.state.treeElements;
        return nodes.filter(node => node.parentNode == null);
    }

    getChildNodes = (nodeId) => {

        let nodes = [...this.state.treeElements];
        let childNodes = nodes.filter(node => node.parentNode === nodeId);

        if (!childNodes || childNodes.length === 0) {
            this.fetchChildNodes(nodeId).then(children => {
                if (children.length > 0)
                    nodes = nodes.concat(children);
                this.setState({ treeElements: nodes, loading: false });
            });
        }

        return childNodes;
    }

    onNodeClick = (nodeId) => {
        const  nodes  = [...this.state.treeElements];
        const i = nodes.findIndex(n => n.id === nodeId);
        nodes[i].open = !nodes[i].open;
        this.setState({ treeElements: nodes });
    }

    onDragNode = (node) => {
        this.setState({ dragNode: node });
    }

    onDropNode = (node) => {
        this.setState({loading: true});
        const dragNode = { ...this.state.dragNode };

        const allNodes = [...this.state.treeElements];
        const i = allNodes.findIndex(n => n.id === dragNode.id);

        allNodes[i].parentNode = node.id;
        this.updateNodeParentId(dragNode.id, node.id).then(() => {
            this.fetchChildNodes(node.id).then(children => {
                if (children.length > 0) {
                    let filtered = allNodes.filter(n => n.parentNode != node.id);
                    filtered = filtered.concat(children);

                    this.setState({ treeElements: filtered, dragNode: null, loading: false });
                }
            });
        });

    }

    renderTreeViewData = () => {
        const nodes = this.getRootNodes();
        return nodes.map(node => {
            
            return (<TreeNode
                key={node.id}
                node={node}
                open={node.open}
                nodeClick={this.onNodeClick}
                getChildNodes={this.getChildNodes}
                onDragNode={this.onDragNode}
                onDropNode={this.onDropNode}
            />);
        });
    }

    render() {
        const loading = this.state.loading ? <Loading /> : null;
        const rootNodes = this.state.loading ? null : this.renderTreeViewData();
        return (
            <div className="over-block">
                <div className="main-block">
                    <div className="tree-header">Files</div>
                    <div className="tree-block">
                        {loading}
                        { rootNodes }
                    </div>
                </div>
            </div>
            );
    }

    async populateTreeViewData() {
        const response = await fetch(`treeview/rootnodes/${this.orderNodesBy}`);
        let data = await response.json();
        data.forEach(node => node.open = true);

        this.setState({ treeElements: data, loading: false });
    }

    async fetchChildNodes(parentId) {
        const childNodesResponse = await fetch(`/treeview/childnodes/${parentId}/${this.orderNodesBy}`);
        const childNodesData = await childNodesResponse.json();
        childNodesData.forEach(n => n.open = false);

        return childNodesData;
    }

    async updateNodeParentId(childId, parentId) {
        await fetch(`/treeview/updatenode/${childId}/${parentId}`, {method: 'POST'});
    }
}

const TreeNode = (props) => {
    let iconClassName = "far fa-file";
    let iconCaret = null;
    if (props.node.type === NodeTypes.Folder) {
        iconClassName = props.open === true ? "far fa-folder-open" : "far fa-folder";
        iconCaret = props.open === true ? <span className="fas fa-caret-down"></span> : <span className="fas fa-caret-right"></span>;
    }

    const nodeClassName = props.node.type === NodeTypes.Folder ? "tree-node-folder" : "tree-node-file";
    const nodeClick = props.node.type === NodeTypes.Folder ? props.nodeClick : undefined;

    let childNodes = null;
    let childNodesComponents = null;

    if (props.node.type === NodeTypes.Folder && props.open === true) {

        childNodes = props.getChildNodes(props.node.id);
        childNodesComponents = childNodes ?
            childNodes.map(childNode => {
                return (
                    <TreeNode
                        key={childNode.id}
                        node={childNode}
                        open={childNode.open}
                        nodeClick={nodeClick}
                        getChildNodes={props.getChildNodes}
                        onDragNode={props.onDragNode}
                        onDropNode={props.onDropNode}
                    />
                );
            }) : null;

    }

    const onDragStart = (event) => {
        event.dataTransfer.effectAllowed = "move";
        props.onDragNode(props.node);
    }

    const dragover_handler = (event) => {
        event.preventDefault();
        event.dataTransfer.dropEffect = "move"
    }

    const onDrop = (event) => {
        event.preventDefault();
        props.onDropNode(props.node);
    }

    let onDropMethod = props.node.type === NodeTypes.Folder ? onDrop : null;
    let onDragOver = props.node.type === NodeTypes.Folder ? dragover_handler : null;

    return (
        <div className={nodeClassName}>
            <div onClick={nodeClick ? () => nodeClick(props.node.id) : null}

                draggable={true}
                onDragStart={onDragStart}
                onDrop={onDropMethod}
                onDragOver={onDragOver}
            >
                {iconCaret}
                <span className={iconClassName}></span>
                {props.node.name}
                <span className="node-size">размер: {props.node.size}</span>
            </div>
            {childNodesComponents}
        </div>
        );
}

const Loading = (props) => {
    return (
        <div className="loading-wrapper">
            <div className="loading-block">
             <img className="loading-image" src={ loading} alt="loading..." />
            </div>
        </div>
        );
}


